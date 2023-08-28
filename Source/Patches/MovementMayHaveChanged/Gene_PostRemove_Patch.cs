using HarmonyLib;
using PathfindingFramework.Cache.Global;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when a gene with a movement extension is removed.
	/// </summary>
	[HarmonyPatch(typeof(Gene), nameof(Gene.PostRemove))]
	internal static class Gene_PostRemove_Patch
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