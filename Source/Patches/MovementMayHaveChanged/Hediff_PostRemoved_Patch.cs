using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.PostRemoved))]
	internal static class Hediff_PostRemoved_Patch
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