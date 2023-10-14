using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	public static class RegionTraverserUtil
	{
		private static RegionEntryPredicate GeneratePredicate(RegionEntryPredicate originalPredicate, Region root)
		{
			return (from, to) => originalPredicate(from, to) && from.TerrainDef() == to.TerrainDef();
		}

		public static IEnumerable<CodeInstruction> PatchRegionMergingPredicate(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo generatePredicateMethod =
				AccessTools.Method(typeof(RegionTraverserUtil),
					nameof(GeneratePredicate));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stloc_1) // entryCondition, both functions share the same position for it.
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // root
					yield return new CodeInstruction(OpCodes.Call, generatePredicateMethod);
				}

				yield return instruction;
			}
		}
	}
}