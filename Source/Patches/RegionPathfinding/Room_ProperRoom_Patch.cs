using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Since all passable regions in the map usually connect to the outside, vanilla can use district types and
	/// "touches map borders" to optimize Room calculation. In Pathfinding Framework, all of the regions of the room must
	/// only link to other rooms in the same district, or Portal regions.
	/// </summary>
	[HarmonyPatch(typeof(Room), nameof(Room.ProperRoom), MethodType.Getter)]
	internal static class Room_ProperRoom_Patch
	{
		private static void Postfix(Room __instance, List<District> ___districts, ref bool __result)
		{
			if (!__result)
			{
				// We only need to check for false positives.
				return;
			}

			if (___districts.Count != 1)
			{
				__result = false;
				return;
			}

			District district = ___districts[0];
			List<Region> regions = district.Regions;
			for (int regionIndex = 0; regionIndex < regions.Count; ++regionIndex)
			{
				Region region = regions[regionIndex];
				if (region.UniqueTerrainDef() != null)
				{
					__result = __instance.PsychologicallyOutdoors;
					break;
				}
			}
		}
	}
}