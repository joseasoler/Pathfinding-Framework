using HarmonyLib;
using PathfindingFramework.PawnMovement;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Pawns that usually avoid fences might bash them instead if they have a canBashFences job. In this case, the
	/// movement context needs to be switched from the ShouldAvoidFences variant to the normal one.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), "CleanupCurrentJob")]
	public static class Pawn_JobTracker_CleanupCurrentJob_Patch
	{
		public static void Prefix(Job ___curJob, out bool __state)
		{
			__state = ___curJob != null && ___curJob.canBashFences;
		}

		public static void Postfix(Pawn ___pawn, Job ___curJob, bool __state)
		{
			bool currentJobCanBashFences = ___curJob != null && ___curJob.canBashFences;
			if (___pawn.Spawned && ___pawn.roping != null && ___pawn.ShouldAvoidFences && currentJobCanBashFences != __state)
			{
				PawnMovementUpdater.Update(___pawn);
			}
		}
	}
}