using HarmonyLib;
using PathfindingFramework.PawnGraphic;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Force a PawnGraphicSet update when the locomotion urgency of a pawn changes. 
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), "CleanupCurrentJob")]
	internal static class Pawn_JobTracker_CleanupCurrentJob_Patch
	{
		private static void Prefix(Pawn ___pawn, out LocomotionUrgency __state)
		{
			__state = CurrentUrgencyUtil.Get(___pawn);
		}

		private static void Postfix(Pawn ___pawn, LocomotionUrgency __state)
		{
			if (__state != CurrentUrgencyUtil.Get(___pawn))
			{
				___pawn.GraphicContext()?.LocomotionUpdated();
			}
		}
	}
}