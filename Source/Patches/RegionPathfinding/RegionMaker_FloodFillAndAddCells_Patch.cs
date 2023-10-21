using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Impassable regions are split depending on the TerrainDef of their cells.
	/// This guarantees that each impassable region is composed of a single terrain type, making possible to check if
	/// a region should be impassable for a specific movement type.
	/// </summary>
	[HarmonyPatch(typeof(RegionMaker), "FloodFillAndAddCells")]
	internal static class RegionMaker_FloodFillAndAddCells_Patch
	{
		private static bool TerrainsShouldBelongToSameRegion(TerrainDef lhs, TerrainDef rhs)
		{
			if (lhs.passability != rhs.passability)
			{
				// Two regions must never contain terrains with different vanilla passability values.
				return false;
			}

			if (lhs.passability == Traversability.Standable)
			{
				// Vanilla case; all standable cells get grouped together.
				return true;
			}

			// In the case of impassable terrains, they can only be grouped together with the same terrain.
			return lhs == rhs;
		}

		private static Predicate<IntVec3> ReplaceRegionFloodFillPredicate(Predicate<IntVec3> original,
			IntVec3 root, Map map)
		{
			// RegionTypeUtility_GetExpectedRegionType_Patch makes sure that regions with impassable terrains that are made
			// passable by any movement type are considered normal region. To avoid pathfinding issues in other movement
			// types, terrains impassable in vanilla must be grouped together in regions without any other terrains.
			TerrainDef rootTerrain = root.GetTerrain(map);
			return cell => original(cell) && TerrainsShouldBelongToSameRegion(rootTerrain, cell.GetTerrain(map));
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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