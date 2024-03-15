using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.CacheLifeCycle
{
	/// <summary>
	/// Remove all map path cost information when a map is removed from the game.
	/// </summary>
	[HarmonyPatch(typeof(Game), nameof(Game.DeinitAndRemoveMap_NewTemp))]
	public static class Game_DeinitAndRemoveMap_NewTemp_Patch
	{
		public static void Prefix(Map map)
		{
			// Stop accepting pathfinding update calls.
			map.MapPathCostGrid() = null;
			map.MovementContextData() = null;
		}
	}
}