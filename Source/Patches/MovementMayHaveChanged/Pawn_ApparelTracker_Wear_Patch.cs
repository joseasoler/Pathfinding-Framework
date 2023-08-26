using HarmonyLib;
using PathfindingFramework.Cache;
using PathfindingFramework.Cache.Global;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
	internal static class Pawn_ApparelTracker_Wear_Patch
	{
		internal static void Postfix(Pawn ___pawn, Apparel newApparel)
		{
			if (___pawn.Spawned && MovementExtensionCache.Contains(newApparel.def))
			{
				PawnMovementCache.Update(___pawn);
			}
		}
	}
}