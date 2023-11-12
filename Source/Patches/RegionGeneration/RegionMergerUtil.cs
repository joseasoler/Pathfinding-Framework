using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	public static class RegionMergerUtil
	{
		private static RegionEntryPredicate GeneratePredicate(RegionEntryPredicate originalPredicate)
		{
			const RegionType combinableRegion = RegionType.Fence | RegionType.Normal;

			// For vanilla-like regions, their TerrainDef call will return null.
			// For regions composed of terrains that must have unique regions, this returns their terrain.
			return (from, to) => originalPredicate(from, to) &&
				((to.type & combinableRegion) == 0 || from.UniqueTerrainDef() == to.UniqueTerrainDef());
		}

		public static IEnumerable<CodeInstruction> PatchRegionMergingPredicate(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo generatePredicateMethod =
				AccessTools.Method(typeof(RegionMergerUtil),
					nameof(GeneratePredicate));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stloc_1) // entryCondition, both functions share the same position for it.
				{
					yield return new CodeInstruction(OpCodes.Call, generatePredicateMethod);
				}

				yield return instruction;
			}
		}
	}
}