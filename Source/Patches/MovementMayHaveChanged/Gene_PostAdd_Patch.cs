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
	public static class Gene_PostAdd_Patch
	{
		public static void Postfix(Gene __instance)
		{
			if (__instance.pawn.Spawned && MovementDefDatabase<GeneDef>.Get(__instance.def) != null)
			{
				PawnMovementUpdater.Update(__instance.pawn);
			}
		}
	}
}