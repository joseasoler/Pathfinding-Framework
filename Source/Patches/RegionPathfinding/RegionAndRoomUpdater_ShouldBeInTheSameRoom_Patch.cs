﻿using System.Linq;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Patches district generation to take into account the changes from RegionMaker_TryGenerateRegionFrom_Patch.
	/// Districts can only be merged if their regions have the same TerrainDef.
	/// </summary>
	[HarmonyPatch(typeof(RegionAndRoomUpdater), "ShouldBeInTheSameRoom")]
	internal static class RegionAndRoomUpdater_ShouldBeInTheSameRoom_Patch
	{
		internal static void Postfix(ref bool __result, District a, District b)
		{
			if (!__result || a.Regions.Count == 0 || b.Regions.Count == 0)
			{
				return;
			}

			__result = a.Regions[0].UniqueTerrainDef() == b.Regions[0].UniqueTerrainDef();
		}
	}
}