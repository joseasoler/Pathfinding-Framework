using HarmonyLib;
using PathfindingFramework.Cache;
using PathfindingFramework.Cache.Global;
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
				PawnMovementCache.Update(__instance.pawn);
			}
		}
	}
}