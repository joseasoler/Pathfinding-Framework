using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.TerrainCostGrid
{
	/// <summary>
	/// Handle updates to the terrain cost grids.
	/// </summary>
	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	internal static class TerrainGrid_DoTerrainChangedEffects_Patch
	{
		internal static void Postfix(Map ___map, IntVec3 c)
		{
			if (___map.uniqueID < 0)
			{
				// m00nl1ght.MapPreview uses maps without uniqueID to generate previews.
				return;
			}

			MapPathCostCache.GetCache(___map.uniqueID).UpdateTerrainCost(c);
		}
	}
}