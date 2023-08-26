using HarmonyLib;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Remove the pawn from the PawnMovementCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
	internal static class Pawn_DeSpawn_Patch
	{
		internal static void Postfix(Pawn __instance)
		{
			PawnMovementCache.Remove(__instance);
		}
	}
}