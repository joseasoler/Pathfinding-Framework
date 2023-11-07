using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Set the TerrainDef for each region composed of impassable terrain that can be walked by some movement type.
	/// Their type is already set to normal due to RegionTypeUtility_GetExpectedRegionType_Patch.
	/// This is required because most region pathfinding functions (including but not limited to RegionTraverser)
	/// use the region type to filter impassable regions out of pathfinding.
	/// </summary>
	[HarmonyPatch(typeof(RegionMaker), nameof(RegionMaker.TryGenerateRegionFrom))]
	internal static class RegionMaker_TryGenerateRegionFrom_Patch
	{
		internal static void Postfix(IntVec3 root, Region __result)
		{
			if (__result?.type == RegionType.Normal)
			{
				TerrainDef terrainDef = root.GetTerrain(__result.Map);
				if (
					// terrainDef can be null when generating a new map and playing with Dubs Performance Analyzer. See #94.
					terrainDef != null &&
					terrainDef.ExtendedRegionType() > 0)
				{
					__result.UniqueTerrainDef() = terrainDef;
				}
			}
		}
	}
}