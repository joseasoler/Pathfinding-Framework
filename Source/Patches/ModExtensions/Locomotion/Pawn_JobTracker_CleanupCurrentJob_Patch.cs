using HarmonyLib;
using PathfindingFramework.PawnGraphic;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Force a graphics update when the locomotion urgency of a pawn changes. 
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), "CleanupCurrentJob")]
	public static class Pawn_JobTracker_CleanupCurrentJob_Patch
	{
		public static void Prefix(Pawn ___pawn, out LocomotionUrgency __state)
		{
			__state = CurrentUrgencyUtil.Get(___pawn);
		}

		public static void Postfix(Pawn ___pawn, LocomotionUrgency __state)
		{
			if (__state != CurrentUrgencyUtil.Get(___pawn))
			{
				___pawn.GraphicContext()?.LocomotionUpdated();
			}
		}
	}
}