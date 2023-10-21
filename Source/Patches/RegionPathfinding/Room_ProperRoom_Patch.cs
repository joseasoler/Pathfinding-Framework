using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Rooms composed only of regions which were impassable in vanilla should not be considered proper rooms.
	/// </summary>
	[HarmonyPatch(typeof(Room), nameof(Room.ProperRoom), MethodType.Getter)]
	internal static class Room_ProperRoom_Patch
	{
		private static void Postfix(List<District> ___districts, ref bool __result)
		{
			if (!__result)
			{
				return;
			}

			bool allImpassableInVanilla = true;
			for (int districtIndex = 0; districtIndex < ___districts.Count; ++districtIndex)
			{
				District district = ___districts[districtIndex];
				List<Region> regions = district.Regions;
				for (int regionIndex = 0; regionIndex < regions.Count; ++regionIndex)
				{
					Region region = regions[regionIndex];
					if (region.TerrainDef() == null)
					{
						allImpassableInVanilla = false;
						break;
					}
				}

				if (allImpassableInVanilla)
				{
					break;
				}
			}

			__result &= !allImpassableInVanilla;
		}
	}
}