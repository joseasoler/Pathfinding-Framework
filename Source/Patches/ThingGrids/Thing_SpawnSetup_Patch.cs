using HarmonyLib;
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
				__instance.Map.MapPathCostGrid().UpdateThings(__instance.Position);
			}
		}
	}
}