using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.TerrainChanges
{
	/// <summary>
	/// Handle updates to the terrain path cost grids.
	/// Trigger region updates when necessary.
	/// </summary>
	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	public static class TerrainGrid_DoTerrainChangedEffects_Patch
	{
		public static void Postfix(Map ___map, IntVec3 c)
		{
			if (___map == null)
			{
				return;
			}

			___map.MovementContextData()?.UpdateCell(c);

			// PathGrid.RecalculatePerceivedPathCostAt uses the vanilla path grid to determine when the regions
			// should be updated. These extra checks take care of PF specific cases.
			TerrainDef terrainDef = c.GetTerrain(___map);
			TerrainDef regionTerrainDef = ___map.regionGrid.GetRegionAt_NoRebuild_InvalidAllowed(c)?.UniqueTerrainDef();
			int extendedRegionType = regionTerrainDef?.ExtendedRegionType() ?? 0;
			if (extendedRegionType != terrainDef.ExtendedRegionType())
			{
				___map.regionDirtyer.Notify_WalkabilityChanged(c, true);
			}
		}
	}
}