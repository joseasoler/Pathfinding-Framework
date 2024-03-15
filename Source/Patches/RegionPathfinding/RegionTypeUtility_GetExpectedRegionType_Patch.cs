using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Set impassable regions that can be walked by one of the defined movement types as passable.
	/// This is required because most region pathfinding functions (including but not limited to RegionTraverser)
	/// use the region type to filter impassable regions out of pathfinding entirely.
	///
	/// This means that region based pathfinding will require additional checks than in vanilla. Patches dealing with
	/// checking region allowance based on movement type will need to take this into account and perform their checks as
	/// early as possible.
	///
	/// In almost every case Region.Allows is used for this, and this function is already patched for it.
	///
	/// RegionMaker_FloodFillAndAddCells_Patch will then take care of the rest of the conditions that these regions
	/// must follow.
	/// </summary>
	[HarmonyPatch(typeof(RegionTypeUtility), nameof(RegionTypeUtility.GetExpectedRegionType))]
	public static class RegionTypeUtility_GetExpectedRegionType_Patch
	{
		public static void Postfix(ref RegionType __result, IntVec3 c, Map map)
		{
			bool hasExtendedRegionType = c.InBounds(map) && c.GetTerrain(map).ExtendedRegionType() > 0;
			if (!hasExtendedRegionType)
			{
				return;
			}

			if (__result == RegionType.ImpassableFreeAirExchange)
			{
				__result = RegionType.Normal;
			}
			else if (__result == RegionType.None)
			{
				// When a passable thing with a fillPercent of 1 is on top of a cell, the vanilla code gives RegionType.Normal.
				// This needs to be implemented here too, otherwise cells with a terrain impassable for terrestrial pawns
				// and a thing on top of them do not get a region assigned.
				List<Thing> thingList = c.GetThingList(map);
				for (int index = 0; index < thingList.Count; ++index)
				{
					Thing thing = thingList[index];
					if (thing.def.Fillage == FillCategory.Full && thing.def.passability != Traversability.Impassable)
					{
						__result = RegionType.Normal;
						break;
					}
				}
			}
		}
	}
}