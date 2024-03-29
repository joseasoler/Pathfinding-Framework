﻿using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	/// <summary>
	/// RegionAndRoomUpdater.FindCurrentRegionGroupNeighborWithMostRegions uses this method to identify neighbors to
	/// unify into a single district.
	/// This patch is required to make district generation take into account the changes from
	/// RegionMaker_TryGenerateRegionFrom_Patch and TerrainRegionType.
	/// </summary>
	[HarmonyPatch(typeof(Region), nameof(Region.NeighborsOfSameType), MethodType.Getter)]
	public static class Region_NeighborsOfSameType_Patch
	{
		public static IEnumerable<Region> Postfix(IEnumerable<Region> __result, Region __instance)
		{
			TerrainDef terrainDef = __instance.UniqueTerrainDef();
			foreach (Region region in __result)
			{
				if (region.UniqueTerrainDef() == terrainDef)
				{
					yield return region;
				}
			}
		}
	}
}