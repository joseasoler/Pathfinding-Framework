using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.TerrainChanges
{
	/// <summary>
	/// Handle updates to the terrain path cost grids.
	/// </summary>
	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	internal static class TerrainGrid_DoTerrainChangedEffects_Patch
	{
		internal static void Postfix(Map ___map, IntVec3 c)
		{
			___map.MovementContextData().UpdateCell(c);
		}
	}
}