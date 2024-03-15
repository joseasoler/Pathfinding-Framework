using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using RimWorld;

namespace PathfindingFramework.Patches.HasDoor
{
	/// <summary>
	/// Keep door pathfinding information updated.
	/// </summary>
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.SpawnSetup))]
	public static class Building_Door_SpawnSetup_Patch
	{
		public static void Postfix(Building_Door __instance)
		{
			__instance.Map.MapPathCostGrid().SetHasDoor(__instance.Position, true);
		}
	}
}