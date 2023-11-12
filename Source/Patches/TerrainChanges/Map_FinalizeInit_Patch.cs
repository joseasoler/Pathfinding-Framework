using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.TerrainChanges
{
	/// <summary>
	/// Initialize movement context handling after the map is fully created or loaded.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	public static class Map_FinalizeInit_Patch
	{
		public static void Prefix(Map __instance)
		{
			__instance.MapPathCostGrid().UpdateSnowAllCells();
			__instance.MovementContextData().UpdateAllCells();
		}
	}
}