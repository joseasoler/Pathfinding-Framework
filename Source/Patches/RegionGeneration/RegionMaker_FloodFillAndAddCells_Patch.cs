using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	/// <summary>
	/// Certain terrains must be in regions that only contain the same terrain, as described in TerrainRegionType.
	/// </summary>
	[HarmonyPatch(typeof(RegionMaker), "FloodFillAndAddCells")]
	public static class RegionMaker_FloodFillAndAddCells_Patch
	{
		private static bool TerrainsShouldBelongToSameRegion(TerrainDef lhs, TerrainDef rhs)
		{

			return
				// terrainDef can be null when generating a new map and playing with Dubs Performance Analyzer. See #94.
				lhs != null && rhs != null &&
				// Two regions must never contain terrains with different vanilla passability values.
				// This check is necessary because regions of impassable terrain might have a normal RegionType if they can be
				// traversed by at least one movement type.
				lhs.passability == rhs.passability &&
				// See TerrainRegionType for details.
				lhs.ExtendedRegionType() == rhs.ExtendedRegionType();
		}

		public static Predicate<IntVec3> ReplaceRegionFloodFillPredicate(Predicate<IntVec3> original,
			IntVec3 root, Map map)
		{
			// RegionTypeUtility_GetExpectedRegionType_Patch makes sure that regions with impassable terrains that are made
			// passable by any movement type are considered normal region. To avoid pathfinding issues in other movement
			// types, terrains impassable in vanilla must be grouped together in regions without any other terrains.
			TerrainDef rootTerrain = root.GetTerrain(map);
			return cell => original(cell) && TerrainsShouldBelongToSameRegion(rootTerrain, cell.GetTerrain(map));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo replaceRegionFloodFillPredicateMethod =
				AccessTools.Method(typeof(RegionMaker_FloodFillAndAddCells_Patch), nameof(ReplaceRegionFloodFillPredicate));
			FieldInfo mapField = AccessTools.Field(typeof(RegionMaker), "map");

			int foundCount = 0;
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;

				if (instruction.opcode != OpCodes.Newobj ||
				    instruction.operand is not ConstructorInfo constructorInfo ||
				    constructorInfo.DeclaringType != typeof(Predicate<IntVec3>))
				{
					continue;
				}

				++foundCount;
				if (foundCount > 1)
				{
					Report.Error($"{nameof(RegionMaker_FloodFillAndAddCells_Patch)} found two different predicates!");
					continue;
				}

				// Root cell additional parameter
				yield return new CodeInstruction(OpCodes.Ldarg_1); // root
				// Map additional parameter.
				yield return new CodeInstruction(OpCodes.Ldarg_0); // this
				yield return new CodeInstruction(OpCodes.Ldfld, mapField); // Verse.RegionMaker::map
				// Inject call to the delegate replacer.
				yield return new CodeInstruction(OpCodes.Call, replaceRegionFloodFillPredicateMethod);
			}

			if (foundCount == 0)
			{
				Report.Error($"{nameof(RegionMaker_FloodFillAndAddCells_Patch)} could not find the predicate!");
			}
		}
	}
}