using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Set the TerrainDef for each impassable region.
	/// Set impassable regions that can be walked by one of the defined movement types as passable.
	/// This is required because most region pathfinding functions (including but not limited to RegionTraverser)
	/// use the region type to filter impassable regions out of pathfinding.
	/// This means that region based pathfinding will require additional checks that in vanilla. Patches dealing with
	/// checking region allowance based on movement type will need to take this into account and perform their checks as
	/// early as possible.
	/// </summary>
	[HarmonyPatch(typeof(RegionMaker), nameof(RegionMaker.TryGenerateRegionFrom))]
	internal static class RegionMaker_TryGenerateRegionFrom_Patch
	{
		internal static void Postfix(IntVec3 root, Region __result)
		{
			if (__result?.type == RegionType.ImpassableFreeAirExchange)
			{
				TerrainDef terrainDef = root.GetTerrain(__result.Map);
				__result.TerrainDef() = terrainDef;
				if (terrainDef.PassableWithAnyMovement())
				{
					__result.type = RegionType.Normal;
				}
			}
		}
	}
}