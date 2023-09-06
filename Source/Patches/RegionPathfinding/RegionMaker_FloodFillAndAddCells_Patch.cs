using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	[HarmonyPatch(typeof(RegionMaker), "FloodFillAndAddCells")]
	internal static class RegionMaker_FloodFillAndAddCells_Patch
	{
		private static Predicate<IntVec3> ReplaceRegionFloodFillPredicate(Predicate<IntVec3> original, RegionType type,
			IntVec3 root, Map map)
		{
			if (type != RegionType.ImpassableFreeAirExchange)
			{
				return original;
			}

			TerrainDef terrainDef = root.GetTerrain(map);
			return cell => original(cell) && cell.GetTerrain(map) == terrainDef;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo replaceRegionFloodFillPredicateMethod =
				AccessTools.Method(typeof(RegionMaker_FloodFillAndAddCells_Patch), nameof(ReplaceRegionFloodFillPredicate));
			FieldInfo newRegField = AccessTools.Field(typeof(RegionMaker), "newReg");
			FieldInfo regionTypeField = AccessTools.Field(typeof(Region), nameof(Region.type));
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

				// RegionType additional parameter
				yield return new CodeInstruction(OpCodes.Ldarg_0); // this
				yield return new CodeInstruction(OpCodes.Ldfld, newRegField); // Verse.RegionMaker::newReg
				yield return new CodeInstruction(OpCodes.Ldfld, regionTypeField); // Verse.Region::'type'
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