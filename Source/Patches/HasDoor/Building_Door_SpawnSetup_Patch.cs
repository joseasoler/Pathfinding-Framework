using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.HasDoor
{
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.SpawnSetup))]
	internal static class Building_Door_SpawnSetup_Patch
	{
		internal static void Postfix(Building_Door __instance)
		{
			MapPathCostCache.Get(__instance.Map.uniqueID).SetHasDoor(__instance.Position, true);
		}
	}
}