using HarmonyLib;
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
	internal static class RegionTypeUtility_GetExpectedRegionType_Patch
	{
		internal static void Postfix(ref RegionType __result, IntVec3 c, Map map)
		{
			if (__result == RegionType.ImpassableFreeAirExchange && c.GetTerrain(map).PassableWithAnyMovement())
			{
				__result = RegionType.Normal;
			}
		}
	}
}