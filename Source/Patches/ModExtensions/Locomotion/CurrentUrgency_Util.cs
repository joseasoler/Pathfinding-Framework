using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Common implementation for obtaining the current locomotion urgency of a pawn.
	/// </summary>
	public static class CurrentUrgency_Util
	{
		public static LocomotionUrgency Get(Pawn pawn)
		{
			// See Pawn_PathFollower.CostToMoveIntoCell.
			Pawn locomotionUrgencySameAs = pawn.jobs?.curDriver?.locomotionUrgencySameAs;
			if (locomotionUrgencySameAs?.CurJob != null && locomotionUrgencySameAs != pawn &&
			    locomotionUrgencySameAs.Spawned)
			{
				return locomotionUrgencySameAs.CurJob.locomotionUrgency;
			}

			return pawn.CurJob?.locomotionUrgency ?? LocomotionUrgency.Amble;
		}
	}
}