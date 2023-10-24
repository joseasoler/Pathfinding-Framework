using HarmonyLib;
using PathfindingFramework.PawnGraphic;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Inform the graphic context of locomotion urgency changes of its pawn.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
	internal static class Pawn_JobTracker_StartJob_Patch
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