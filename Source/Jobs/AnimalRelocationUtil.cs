using System;
using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapComponents.MovementContexts;
using PathfindingFramework.Patches;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Jobs
{
	internal struct DestinationData
	{
		public readonly float score;
		public readonly IntVec3 cell;

		public DestinationData(float sc, IntVec3 cll)
		{
			score = sc;
			cell = cll;
		}
	}

	/// <summary>
	/// Utility class for the wild animal relocation feature.
	/// </summary>
	public static class AnimalRelocationUtil
	{
		private const int MinimumDistrictSize = 2 * Region.GridSize * Region.GridSize;

		/// <summary>
		/// Checks if a recently spawned animal group should relocate to a better district.
		/// </summary>
		/// <param name="animalGroup">Group to check.</param>
		/// <returns>True if the group should relocate.</returns>
		private static bool ShouldRelocate(List<Pawn> animalGroup)
		{
			if (!Settings.Values.WildAnimalRelocating || animalGroup.Count == 0)
			{
				return false;
			}

			foreach (Pawn animal in animalGroup)
			{
				MovementContext context = animal.MovementContext();
				if (context.AvoidWanderAt(animal.Position))
				{
					return true;
				}
			}

			District district = animalGroup[0].GetDistrict();
			return district.CellCount < MinimumDistrictSize;
		}

		/// <summary>
		/// Helper function for a movement-aware wander check.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="pawn"></param>
		/// <returns></returns>
		private static bool CanWanderToCell(IntVec3 destination, Pawn pawn)
		{
			MovementContext context = pawn.MovementContext();
			if (!context.CanStandAt(destination) || context.AvoidWanderAt(destination))
			{
				return false;
			}

			// Movement type aware checks for standability and wander have already been performed.
			// A tryIndex value above 10 will make the vanilla code ignore these.
			const int tryIndex = 11;
			return RCellFinder.CanWanderToCell(destination, pawn, pawn.Position, null, tryIndex, Danger.None, false, false,
				false, false);
		}

		private static List<DestinationData> GetRelocationDestinations(List<Pawn> animalGroup)
		{
			Pawn animal = animalGroup[0];
			Map map = animal.Map;
			District originalDistrict = animal.GetDistrict();
			IntVec3 originalPosition = animal.Position;

			List<IntVec3> points = new List<IntVec3>();
			List<int> distances = new List<int>();
			float maxDistance = float.MinValue;
			List<int> sizes = new List<int>();
			float maxSize = float.MinValue;
			foreach (District targetDistrict in map.regionGrid.allDistricts)
			{
				int size = targetDistrict.CellCount;
				if (originalDistrict == targetDistrict || targetDistrict.Regions.Count == 0 || size < MinimumDistrictSize)
				{
					continue;
				}

				Region targetRegion = targetDistrict.Regions.RandomElement();
				if (targetRegion == null || targetRegion.type != RegionType.Normal || targetRegion.IsDoorway)
				{
					continue;
				}

				IntVec3 destination = targetRegion.AnyCell;
				if (!CanWanderToCell(destination, animal))
				{
					continue;
				}

				points.Add(destination);

				int distance = (originalPosition - destination).LengthManhattan;
				distances.Add(distance);
				maxDistance = Math.Max(maxDistance, distance);

				sizes.Add(size);
				maxSize = Math.Max(maxSize, size);
			}

			List<DestinationData> destinations = new List<DestinationData>();
			for (int index = 0; index < points.Count; ++index)
			{
				float score = distances[index] / maxDistance + sizes[index] / maxSize;
				destinations.Add(new DestinationData(score, points[index]));
			}

			return destinations;
		}

		public static void HandleGroup(List<Pawn> animalGroup)
		{
			if (!ShouldRelocate(animalGroup))
			{
				return;
			}

			List<DestinationData> destinations = GetRelocationDestinations(animalGroup);
			if (!destinations.TryRandomElementByWeight(dest => dest.score, out var destination))
			{
				return;
			}

			IntVec3 cell = destination.cell;

			foreach (Pawn animal in animalGroup)
			{
				if (!LocationFinding.TryRandomClosewalkCellNearPos(cell, animal, 6, out IntVec3 animalDestination))
				{
					continue;
				}

				Job job = JobMaker.MakeJob(RimWorld.JobDefOf.GotoWander, animalDestination, 20000);
				job.locomotionUrgency = LocomotionUrgency.Amble;
				animal.jobs.StartJob(job);
			}
		}
	}
}