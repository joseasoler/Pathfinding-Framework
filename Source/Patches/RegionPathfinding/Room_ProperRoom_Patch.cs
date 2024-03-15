using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Rooms composed of a single district with a terrain impassable for vanilla pathfindings cannot be considered
	/// proper rooms.
	/// </summary>
	[HarmonyPatch(typeof(Room), nameof(Room.ProperRoom), MethodType.Getter)]
	public static class Room_ProperRoom_Patch
	{
		public static void Postfix(Room __instance, List<District> ___districts, ref bool __result)
		{
			if (!__result || ___districts.Count > 1)
			{
				return;
			}

			TerrainDef terrainDef = ___districts[0].Regions[0].UniqueTerrainDef();
			if (terrainDef != null)
			{
				__result = MovementDefOf.PF_Movement_Terrestrial.CanEnterTerrain(terrainDef);
			}
		}
	}
}