using HarmonyLib;
using PathfindingFramework.MapPathCosts;
using Verse;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// Initialize a new MapPathCostGrid when a map is created.
	/// It is created as early as possible in the map life cycle to avoid the need for presence checks.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.ConstructComponents))]
	internal static class Map_ConstructComponents_Patch
	{
		internal static void Prefix(Map __instance)
		{
			if (__instance.uniqueID < 0)
			{
				// m00nl1ght.MapPreview uses maps without uniqueID to generate previews.
				return;
			}

			// Start accepting pathfinding update calls after the map is fully initialized.
			__instance.MapPathCostGrid() = new MapPathCostGrid(__instance);
		}
	}
}