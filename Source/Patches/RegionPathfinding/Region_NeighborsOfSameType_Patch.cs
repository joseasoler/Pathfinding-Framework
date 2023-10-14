using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// RegionAndRoomUpdater.FindCurrentRegionGroupNeighborWithMostRegions uses this method to identify neighbors to
	/// unify into a single district.
	/// This patch is required to make district generation take into account the changes from
	/// RegionMaker_TryGenerateRegionFrom_Patch.
	/// </summary>
	[HarmonyPatch(typeof(Region), nameof(Region.NeighborsOfSameType), MethodType.Getter)]
	internal static class Region_NeighborsOfSameType_Patch
	{
		private static IEnumerable<Region> Postfix(IEnumerable<Region> __result, Region __instance)
		{
			TerrainDef terrainDef = __instance.TerrainDef();
			foreach (Region region in __result)
			{
				if (region.TerrainDef() == terrainDef)
				{
					yield return region;
				}
			}
		}
	}
}