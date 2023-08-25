using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Gene), nameof(Gene.PostAdd))]
	internal static class Gene_PostAdd_Patch
	{
		internal static void Postfix(Gene __instance)
		{
			if (__instance.pawn.Spawned && MovementExtensionCache.Contains(__instance.def))
			{
				PawnMovementCache.Recalculate(__instance.pawn);
			}
		}
	}
}