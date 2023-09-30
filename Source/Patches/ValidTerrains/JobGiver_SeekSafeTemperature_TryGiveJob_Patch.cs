using HarmonyLib;
using PathfindingFramework.MovementContexts;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ValidTerrains
{
	[HarmonyPatch(typeof(JobGiver_SeekSafeTemperature), "TryGiveJob")]
	internal static class JobGiver_SeekSafeTemperature_TryGiveJob_Patch
	{
		internal static bool Prefix(ref Job __result, Pawn pawn)
		{
			IntVec3 root = pawn.Position;
			MovementContext context = pawn.MovementContext();
			if (context.CanEnterTerrain(root))
			{
				return true;
			}

			IntVec3 targetCell = CloseCellWithSafeTerrain(root, pawn.Map, TraverseParms.For(pawn));
			if (targetCell != IntVec3.Invalid)
			{
				__result = JobMaker.MakeJob(JobDefOf.PF_SeekSafeTerrain, targetCell, 300);
				return false;
			}

			return true;
		}

		private static IntVec3 CloseCellWithSafeTerrain(IntVec3 root, Map map, TraverseParms traverseParms)
		{
			IntVec3 foundCell = IntVec3.Invalid;

			Region region = root.GetRegion(map);
			if (region == null)
			{
				return foundCell;
			}

			// Creatures are allowed to traverse regions that should be avoided...
			bool EntryCondition(Region _, Region r) => r.Allows(traverseParms, false);

			bool RegionProcessor(Region r)
			{
				// ... but they cannot choose regions to be avoided as a destination.
				if (r.IsDoorway || !r.Allows(traverseParms, true))
				{
					return false;
				}

				IntVec3 destination = r.AnyCell;
				if (traverseParms.pawn.MovementContext().CanEnterTerrain(destination))
				{
					foundCell = destination;
					return true;
				}

				return false;
			}

			RegionTraverser.BreadthFirstTraverse(region, EntryCondition, RegionProcessor, 9999);
			return foundCell;
		}
	}
}