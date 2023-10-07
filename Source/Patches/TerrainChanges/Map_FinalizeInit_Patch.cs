using HarmonyLib;
using PathfindingFramework.MovementContexts;
using Verse;

namespace PathfindingFramework.Patches.TerrainChanges
{
	/// <summary>
	/// Initialize movement context handling after the map is fully created or loaded.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	internal static class Map_FinalizeInit_Patch
	{
		internal static void Prefix(Map __instance)
		{
			__instance.MapPathCostGrid().UpdateAllSnow();
			__instance.MovementContextData().UpdateAllCells();
		}
	}
}