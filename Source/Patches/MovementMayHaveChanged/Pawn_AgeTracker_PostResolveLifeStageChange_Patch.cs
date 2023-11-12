using HarmonyLib;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when their life stage changes.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.PostResolveLifeStageChange))]
	public static class Pawn_AgeTracker_PostResolveLifeStageChange_Patch
	{
		public static void Postfix(Pawn ___pawn)
		{
			// PostResolveLifeStageChange might get called during the pawn spawning process.
			if (___pawn.Spawned)
			{
				PawnMovementUpdater.Update(___pawn);
			}
		}
	}
}