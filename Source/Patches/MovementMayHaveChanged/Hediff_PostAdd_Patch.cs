using HarmonyLib;
using PathfindingFramework.Cache.Global;
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
			if (__instance.pawn.Spawned && MovementExtensionCache.Contains(__instance.def))
			{
				PawnMovementCache.Update(__instance.pawn);
			}
		}
	}
}