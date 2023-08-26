using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// The MapPathCostCache is initialized as early as possible to avoid the need for presence checks.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.ConstructComponents))]
	public class Map_ConstructComponents_Patch
	{
		internal static void Prefix(Map __instance)
		{
			MapPathCostCache.Add(__instance);
		}
	}
}