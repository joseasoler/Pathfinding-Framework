using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.PostResolveLifeStageChange))]
	internal static class Pawn_AgeTracker_PostResolveLifeStageChange_Patch
	{
		internal static void Postfix(Pawn_AgeTracker __instance, Pawn ___pawn)
		{
			var lifeStageDef = __instance.CurLifeStage;
			if (___pawn.Spawned && lifeStageDef != null && MovementExtensionCache.Contains(lifeStageDef))
			{
				PawnMovementCache.Update(___pawn);
			}
		}
	}
}