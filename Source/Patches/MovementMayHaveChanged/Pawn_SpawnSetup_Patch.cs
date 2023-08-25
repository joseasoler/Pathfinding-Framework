using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	internal static class Pawn_SpawnSetup_Patch
	{
		internal static void Postfix(Pawn __instance)
		{
			PawnMovementCache.Recalculate(__instance);
		}
	}
}