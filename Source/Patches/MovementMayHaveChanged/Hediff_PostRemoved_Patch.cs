using HarmonyLib;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when a hediff with a movement extension is removed.
	/// </summary>
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.PostRemoved))]
	public static class Hediff_PostRemoved_Patch
	{
		public static void Postfix(Hediff __instance)
		{
			if (__instance.pawn.Spawned && MovementDefDatabase<HediffDef>.Get(__instance.def) != null)
			{
				PawnMovementUpdater.Update(__instance.pawn);
			}
		}
	}
}