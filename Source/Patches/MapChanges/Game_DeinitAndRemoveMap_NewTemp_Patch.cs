using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// Remove all map path cost information when a map is removed from the game.
	/// </summary>
	[HarmonyPatch(typeof(Game), nameof(Game.DeinitAndRemoveMap_NewTemp))]
	internal static class Game_DeinitAndRemoveMap_NewTemp_Patch
	{
		internal static void Prefix(Map map)
		{
			// Stop accepting pathfinding update calls.
			map.MapPathCostGrid() = null;
		}
	}
}