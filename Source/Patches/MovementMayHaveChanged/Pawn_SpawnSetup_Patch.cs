using HarmonyLib;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the movement type of the pawn in the PawnMovementCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	internal static class Pawn_SpawnSetup_Patch
	{
		internal static void Postfix(Pawn __instance)
		{
			PawnMovementUpdater.Update(__instance);
		}
	}
}