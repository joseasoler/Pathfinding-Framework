using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	[HarmonyPatch(typeof(Game), nameof(Game.DeinitAndRemoveMap_NewTemp))]
	public class Game_DeinitAndRemoveMap_NewTemp_Patch
	{
		internal static void Prefix(Map map)
		{
			MapPathCostCache.Remove(map);
		}
	}
}