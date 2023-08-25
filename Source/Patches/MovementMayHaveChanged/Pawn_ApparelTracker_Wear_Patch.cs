using HarmonyLib;
using PathfindingFramework.Cache;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
	internal static class Pawn_ApparelTracker_Wear_Patch
	{
		internal static void Postfix(Pawn ___pawn, Apparel newApparel)
		{
			PawnMovementCache.Recalculate(___pawn);
			if (___pawn.Spawned && MovementExtensionCache.Contains(newApparel.def))
			{
				PawnMovementCache.Recalculate(___pawn);
			}
		}
	}
}