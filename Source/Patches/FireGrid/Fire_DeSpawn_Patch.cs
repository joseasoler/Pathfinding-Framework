using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.FireGrid
{
	/// <summary>
	/// Keeps the fire grid updated after a fire is destroyed.
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

			var map = __instance.Map;
			if (map != null)
			{
				MapPathCostCache.GetCache(map.uniqueID).UpdateFire(__instance.Position, false);
			}
		}
	}
}