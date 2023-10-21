using System.Linq;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Patches district generation to take into account the changes from RegionMaker_TryGenerateRegionFrom_Patch.
	/// Districts can only be merged if their regions have the same TerrainDef.
	/// For vanilla-like regions, their TerrainDef call will return null. For regions with impassable terrain, this
	/// returns the terrain.
	/// </summary>
	[HarmonyPatch(typeof(RegionAndRoomUpdater), "ShouldBeInTheSameRoom")]
	internal static class RegionAndRoomUpdater_ShouldBeInTheSameRoom_Patch
	{
		internal static void Postfix(ref bool __result, District a, District b)
		{
			if (!__result || a.Regions.Count == 0 || b.Regions.Count == 0)
			{
				return;
			}

			__result = a.Regions[0].TerrainDef() == b.Regions[0].TerrainDef();
		}
	}
}