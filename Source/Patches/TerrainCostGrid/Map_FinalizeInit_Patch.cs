﻿using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.TerrainCostGrid
{
	/// <summary>
	/// Initialize required terrain path grids after the map is fully created or loaded.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	public class Map_FinalizeInit_Patch
	{
		internal static void Prefix(Map __instance)
		{
			MapPathCostCache.GetCache(__instance.uniqueID).UpdateAllTerrainCosts();
		}
	}
}