using HarmonyLib;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when a hediff with a movement extension is added.
	/// </summary>
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.PostAdd))]
	public static class Hediff_PostAdd_Patch
	{
		public static void Postfix(Hediff __instance)
		{
			if (__instance.pawn.Spawned && __instance.def.MovementDef() != null)
			{
				PawnMovementUpdater.Update(__instance.pawn);
			}
		}
	}
}