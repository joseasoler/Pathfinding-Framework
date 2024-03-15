using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	/// <summary>
	/// Two districts can be added to the same room if both of them are passable by the terrestrial movement type (in
	/// other words, both of them would be passable in vanilla) or if they have exactly the same UniqueTerrainDef.
	/// </summary>
	[HarmonyPatch(typeof(RegionAndRoomUpdater), "ShouldBeInTheSameRoom")]
	public static class RegionAndRoomUpdater_ShouldBeInTheSameRoom_Patch
	{
		public static void Postfix(ref bool __result, District a, District b)
		{
			if (!__result || a.Regions.Count == 0 || b.Regions.Count == 0)
			{
				return;
			}

			TerrainDef terrainDefA = a.Regions[0].UniqueTerrainDef();
			bool aIsPassableByTerrestrial =
				terrainDefA == null || MovementDefOf.PF_Movement_Terrestrial.CanEnterTerrain(terrainDefA);

			TerrainDef terrainDefB = b.Regions[0].UniqueTerrainDef();
			bool bIsPassableByTerrestrial =
				terrainDefB == null || MovementDefOf.PF_Movement_Terrestrial.CanEnterTerrain(terrainDefB);

			__result = aIsPassableByTerrestrial && bIsPassableByTerrestrial ||
				a.Regions[0].UniqueTerrainDef() == b.Regions[0].UniqueTerrainDef();
		}
	}
}