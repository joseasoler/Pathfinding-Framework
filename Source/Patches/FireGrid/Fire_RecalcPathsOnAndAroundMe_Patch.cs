using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;

namespace PathfindingFramework.Patches.FireGrid
{
	/// <summary>
	/// Keeps the fire grid updated.
	/// </summary>
	[HarmonyPatch(typeof(Fire), "RecalcPathsOnAndAroundMe")]
	internal static class Fire_RecalcPathsOnAndAroundMe_Patch
	{
		internal static void Prefix(Fire __instance)
		{
			var map = __instance.Map;
			if (map != null)
			{
				MapPathCostCache.Get(map.uniqueID).UpdateFire(__instance.Position, __instance.Spawned);
			}
		}
	}
}