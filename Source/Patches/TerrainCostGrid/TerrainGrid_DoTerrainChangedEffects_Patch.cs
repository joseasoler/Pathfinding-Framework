﻿using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.TerrainCostGrid
{
	/// <summary>
	/// Handle updates to the terrain cost grids.
	/// </summary>
	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	public class TerrainGrid_DoTerrainChangedEffects_Patch
	{
		internal static void Postfix(Map ___map, IntVec3 c)
		{
			MapPathCostCache.Get(___map.uniqueID).UpdateTerrainCost(c);
		}
	}
}