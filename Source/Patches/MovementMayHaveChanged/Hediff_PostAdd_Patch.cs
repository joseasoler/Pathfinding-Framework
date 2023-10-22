using HarmonyLib;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when a hediff with a movement extension is added.
	/// </summary>
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.PostAdd))]
	internal static class Hediff_PostAdd_Patch
	{
		internal static void Postfix(Hediff __instance)
		{
			if (__instance.pawn.Spawned && __instance.def.MovementDef() != null)
			{
				PawnMovementUpdater.Update(__instance.pawn);
			}
		}
	}
}