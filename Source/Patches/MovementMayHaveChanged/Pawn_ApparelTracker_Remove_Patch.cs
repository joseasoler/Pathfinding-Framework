using HarmonyLib;
using PathfindingFramework.Cache;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Remove))]
	internal static class Pawn_ApparelTracker_Remove_Patch
	{
		internal static void Postfix(Pawn ___pawn, Apparel ap)
		{
			PawnMovementCache.Recalculate(___pawn);
			if (___pawn.Spawned && MovementExtensionCache.Contains(ap.def))
			{
				PawnMovementCache.Recalculate(___pawn);
			}
		}
	}
}