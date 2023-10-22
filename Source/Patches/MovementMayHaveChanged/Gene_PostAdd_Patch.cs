using HarmonyLib;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when a gene with a movement extension is added.
	/// </summary>
	[HarmonyPatch(typeof(Gene), nameof(Gene.PostAdd))]
	internal static class Gene_PostAdd_Patch
	{
		internal static void Postfix(Gene __instance)
		{
			if (__instance.pawn.Spawned && __instance.def.MovementDef() != null)
			{
				PawnMovementUpdater.Update(__instance.pawn);
			}
		}
	}
}