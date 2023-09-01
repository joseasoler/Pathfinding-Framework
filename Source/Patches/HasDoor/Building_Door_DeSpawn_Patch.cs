using HarmonyLib;
using RimWorld;

namespace PathfindingFramework.Patches.HasDoor
{
	/// <summary>
	/// Keep door pathfinding information updated.
	/// </summary>
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.DeSpawn))]
	internal static class Building_Door_DeSpawn_Patch
	{
		internal static void Prefix(Building_Door __instance)
		{
			__instance.Map.MapPathCostGrid().SetHasDoor(__instance.Position, false);
		}
	}
}