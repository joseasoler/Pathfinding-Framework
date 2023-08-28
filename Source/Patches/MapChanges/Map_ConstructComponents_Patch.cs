using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// Initialize a new MapPathCostCache when a map is created.
	/// It is created as early as possible in the map life cycle to avoid the need for presence checks.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.ConstructComponents))]
	internal static class Map_ConstructComponents_Patch
	{
		internal static void Prefix(Map __instance)
		{
			MapPathCostCache.Add(__instance);
		}
	}
}