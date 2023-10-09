using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Force a PawnGraphicSet update when the locomotion urgency of a pawn changes. 
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
	internal static class Pawn_JobTracker_StartJob_Patch
	{
		private static void Prefix(Pawn ___pawn, out LocomotionUrgency __state)
		{
			__state = CurrentUrgency_Util.Get(___pawn);
		}

		private static void Postfix(Pawn ___pawn, LocomotionUrgency __state)
		{
			if (___pawn.LocomotionExtension()?.graphicData != null && __state != CurrentUrgency_Util.Get(___pawn))
			{
				// Force an update of the PawnGraphicSet.
				___pawn.drawer.renderer.graphics.nakedGraphic = null;
			}
		}
	}
}