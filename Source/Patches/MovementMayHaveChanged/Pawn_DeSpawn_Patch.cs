using HarmonyLib;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Remove the pawn from the PawnMovementCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
	internal static class Pawn_DeSpawn_Patch
	{
		internal static void Prefix(Pawn __instance, out int __state)
		{
			// Pawns lose their Map during the DeSpawning process.
			__state = __instance.Map.uniqueID;
		}

		internal static void Postfix(Pawn __instance, int __state)
		{
			PawnMovementUpdater.Remove(__state, __instance);
		}
	}
}