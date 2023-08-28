using HarmonyLib;
using PathfindingFramework.Cache.Global;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when their life stage changes.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.PostResolveLifeStageChange))]
	internal static class Pawn_AgeTracker_PostResolveLifeStageChange_Patch
	{
		internal static void Postfix(Pawn_AgeTracker __instance, Pawn ___pawn)
		{
			PawnMovementCache.Update(___pawn);
		}
	}
}