using HarmonyLib;
using PathfindingFramework.Cache.Local;
using RimWorld;

namespace PathfindingFramework.Patches.HasDoor
{
	/// <summary>
	/// Keep door pathfinding information updated.
	/// </summary>
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.SpawnSetup))]
	internal static class Building_Door_SpawnSetup_Patch
	{
		internal static void Postfix(Building_Door __instance)
		{
			MapPathCostCache.GetCache(__instance.Map.uniqueID).SetHasDoor(__instance.Position, true);
		}
	}
}