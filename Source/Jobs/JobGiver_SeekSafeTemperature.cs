using PathfindingFramework.MovementContexts;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Jobs
{
	public class JobGiver_SeekSafeTemperature : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			IntVec3 root = pawn.Position;
			MovementContext context = pawn.MovementContext();
			if (context.CanEnterTerrain(root))
			{
				return null;
			}

			IntVec3 targetCell = CloseCellWithSafeTerrain(root, pawn.Map, TraverseParms.For(pawn));
			if (targetCell != IntVec3.Invalid)
			{
				return JobMaker.MakeJob(JobDefOf.PF_Job_SeekSafeTerrain, targetCell, 300);
			}

			return null;
		}
		private static IntVec3 CloseCellWithSafeTerrain(IntVec3 root, Map map, TraverseParms traverseParms)
		{
			IntVec3 foundCell = IntVec3.Invalid;

			Region region = root.GetRegion(map);
			if (region == null)
			{
				return foundCell;
			}

			RegionTraverser.BreadthFirstTraverse(region, EntryCondition, RegionProcessor, 9999);
			return foundCell;


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
				if (!traverseParms.pawn.MovementContext().CanEnterTerrain(destination))
				{
					return false;
				}

				foundCell = destination;
				return true;
			}
		}
	}
}