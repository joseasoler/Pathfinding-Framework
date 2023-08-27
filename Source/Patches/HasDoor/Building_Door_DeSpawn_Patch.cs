using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;

namespace PathfindingFramework.Patches.HasDoor
{
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.DeSpawn))]
	internal static class Building_Door_DeSpawn_Patch
	{
		internal static void Prefix(Building_Door __instance)
		{
			MapPathCostCache.GetCache(__instance.Map.uniqueID).SetHasDoor(__instance.Position, false);
		}
	}
}