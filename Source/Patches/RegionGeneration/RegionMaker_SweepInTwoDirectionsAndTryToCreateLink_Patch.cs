using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	/// <summary>
	/// In vanilla, region edge generation does not take into account the size of the other region.
	/// This happens because in their case it is not necessary, because regions are only grouped by the passability
	/// of their tiles.
	/// With Pathfinding Framework though, it is possible to have adjacent regions of the same passability but with
	/// different edges.
	/// This patch transpiles this method to replace GetExpectedRegionType with a version that returns different values
	/// for cells that should be impassable in vanilla, have been made passable by PF, and have different terrains.
	/// This will make both regions calculate the same length for their edge. This is important because otherwise
	/// EdgeSpan.UniqueHashCode will return different values for each region and they will not be able to match them.
	/// </summary>
	[HarmonyPatch(typeof(RegionMaker), "SweepInTwoDirectionsAndTryToCreateLink")]
	public static class RegionMaker_SweepInTwoDirectionsAndTryToCreateLink_Patch
	{
		public static RegionType GetExpectedRegionTypeExtended(IntVec3 c, Map map)
		{
			const RegionType combinableRegion = RegionType.Fence | RegionType.Normal;
			RegionType regionType = c.GetExpectedRegionType(map);
			if ((regionType & combinableRegion) == 0)
			{
				return regionType;
			}

			TerrainDef terrainDef = c.GetTerrain(map);
			// terrainDef can be null when generating a new map and playing with Dubs Performance Analyzer. See #94.
			if (terrainDef != null)
			{
				regionType += terrainDef.ExtendedRegionType();
			}

			return regionType;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getExpectedRegionTypeOriginalMethod =
				AccessTools.Method(typeof(RegionTypeUtility),
					nameof(RegionTypeUtility.GetExpectedRegionType));

			MethodInfo getExpectedRegionTypeExtendedMethod =
				AccessTools.Method(typeof(RegionMaker_SweepInTwoDirectionsAndTryToCreateLink_Patch),
					nameof(GetExpectedRegionTypeExtended));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(getExpectedRegionTypeOriginalMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, getExpectedRegionTypeExtendedMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}