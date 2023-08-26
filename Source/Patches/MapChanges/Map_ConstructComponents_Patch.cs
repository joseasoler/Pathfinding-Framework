using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	[HarmonyPatch(typeof(Map), nameof(Map.ConstructComponents))]
	public class Map_ConstructComponents_Patch
	{
		internal static void Postfix(Map __instance)
		{
			MapPathCostCache.Add(__instance);
		}
	}
}