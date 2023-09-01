using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.FirePathCost
{
	/// <summary>
	/// Keeps fire pathfinding information updated after a fire is destroyed.
	/// </summary>
	[HarmonyPatch(typeof(Fire), nameof(Fire.DeSpawn))]
	internal static class Fire_DeSpawn_Patch
	{
		internal static void Prefix(Fire __instance)
		{
			if (__instance.parent is Pawn pawn)
			{
				// Ignore fires attached to pawns in pathfinding calculations.
				return;
			}

			__instance.Map?.MapPathCostGrid().UpdateFire(__instance.Position, false);
		}
	}
}