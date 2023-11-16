using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Jobs
{
	using DistrictData = Tuple<District, int, int, IntVec3>;

	/// <summary>
	/// Animals might choose to relocate to other districts.
	/// Larger districts and closer districts are chosen more frequently.
	/// </summary>
	public class JobGiver_RelocateAndWander : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!Settings.Values.WildAnimalRelocation)
			{
				return null;
			}

			District district = pawn.Position.GetDistrict(pawn.Map);
			if (district == null)
			{
				return null;
			}

			IntVec3 targetCell = GetRelocationCell(pawn);

			Job job = targetCell != IntVec3.Invalid
				? JobMaker.MakeJob(RimWorld.JobDefOf.GotoWander, targetCell, 20000)
				: null;

			return job;
		}

		private static DistrictData GetDistrictData(District targetDistrict, Pawn pawn)
		{
			int targetCellCount = targetDistrict.CellCount;

			if (targetDistrict == pawn.Position.GetDistrict(pawn.Map) || targetDistrict.Regions.Count == 0 ||
			    targetCellCount < Region.GridSize * Region.GridSize)
			{
				// Invalid districts, small districts and the same district are ignored.
				return null;
			}

			Region targetRegion = targetDistrict.Regions.RandomElement();
			if (targetRegion == null || targetRegion.type != RegionType.Normal || targetRegion.IsDoorway)
			{
				return null;
			}

			IntVec3 destination = targetRegion.AnyCell;
			// tryIndex intentionally kept at 0 to always perform standability checks.
			if (!RCellFinder.CanWanderToCell(destination, pawn, pawn.Position, null, 0, Danger.None, false, false))
			{
				return null;
			}

			IntVec3 distance = pawn.Position - destination;
			return new DistrictData(targetDistrict, targetCellCount, distance.LengthManhattan, destination);
		}

		private static float GetWeight(DistrictData districtData, District currentDistrict, float maxTargetCount,
			float maxDistance)
		{
			return
				// District cell count weight.
				1.5F * districtData.Item2 / maxTargetCount +
				// Inverse distance weight.
				1.0F - districtData.Item3 / maxDistance +
				// Remain at current district weight.
				(districtData.Item1 == currentDistrict ? 0.5F : 0.0F)
				;
		}

		private static IntVec3 GetRelocationCell(Pawn pawn)
		{
			List<DistrictData> validDistrictData = new List<DistrictData>();
			Map map = pawn.Map;
			float maxTargetCount = float.MinValue;
			float maxDistance = float.MinValue;
			foreach (District district in map.regionGrid.allDistricts)
			{
				DistrictData data = GetDistrictData(district, pawn);
				if (data != null)
				{
					validDistrictData.Add(data);
					maxTargetCount = Math.Max(maxTargetCount, data.Item2);
					maxDistance = Math.Max(maxDistance, data.Item3);
				}
			}

			if (validDistrictData.Count == 0)
			{
				return IntVec3.Invalid;
			}

			DistrictData targetDistrictData = validDistrictData.RandomElementByWeight(data =>
				GetWeight(data, pawn.Position.GetDistrict(pawn.Map), maxTargetCount, maxDistance));

			return targetDistrictData.Item4;
		}
	}
}