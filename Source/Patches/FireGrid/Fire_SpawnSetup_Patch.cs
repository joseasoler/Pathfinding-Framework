using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.FireGrid
{
	/// <summary>
	/// Keeps the fire grid updated after a new fire spawns.
	/// </summary>
	[HarmonyPatch(typeof(Fire), nameof(Fire.SpawnSetup))]
	internal static class Fire_SpawnSetup_Patch
	{
		internal static void Postfix(Fire __instance)
		{
			if (__instance.parent is Pawn pawn)
			{
				// Ignore fires attached to pawns in pathfinding calculations.
				return;
			}

			var map = __instance.Map;
			if (map != null)
			{
				MapPathCostCache.Get(map.uniqueID)?.UpdateFire(__instance.Position, true);
			}
		}
	}
}