using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.ThingGrids
{
	[HarmonyPatch(typeof(Thing), nameof(Thing.SpawnSetup))]
	internal static class Thing_SpawnSetup_Patch
	{
		internal static void Postfix(Thing __instance)
		{
			if (__instance.def.pathCost != 0 || __instance.def.passability == Traversability.Impassable)
			{
				MapPathCostCache.GetCache(__instance.Map.uniqueID).UpdateThings(__instance.Position);
			}
		}
	}
}