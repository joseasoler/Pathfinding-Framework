using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.TerrainCostGrid
{
	/// <summary>
	/// Initialize terrain path grids after the map is fully created or loaded.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	internal static class Map_FinalizeInit_Patch
	{
		internal static void Prefix(Map __instance)
		{
			__instance.MapPathCostGrid().UpdateAllTerrainCosts();
		}
	}
}