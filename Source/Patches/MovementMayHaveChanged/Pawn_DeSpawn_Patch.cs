using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
	internal static class Pawn_DeSpawn_Patch
	{
		internal static void Postfix(Pawn __instance)
		{
			PawnMovementCache.Recalculate(__instance);
		}
	}
}