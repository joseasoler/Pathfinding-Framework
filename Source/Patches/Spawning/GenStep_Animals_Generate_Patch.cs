using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// This is a problematic patch. To ensure mod compatibility, it is not possible to replace GenStep_Animals with a
	/// custom GenStep, as some mods such as Geological Landforms target this with prefixes. Other mods might also use
	/// this GenStep and it would be difficult to catch all of them through XML patching.
	/// Instead, this is transpiling away the RandomAnimalSpawnCell_MapGen call and replacing it with a cell initialized
	/// to Invalid. WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch will then generate a valid cell for the chosen
	/// creature.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_Animals), nameof(GenStep_Animals.Generate))]
	public static class GenStep_Animals_Generate_Patch
	{
		public static IntVec3 GetInvalidCell(Map _)
		{
			return IntVec3.Invalid;
		}

		public static void Prefix()
		{
			WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch.GenerateAnywhere = true;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod =
				AccessTools.Method(typeof(RCellFinder), nameof(RCellFinder.RandomAnimalSpawnCell_MapGen));

			MethodInfo newMethod =
				AccessTools.Method(typeof(GenStep_Animals_Generate_Patch), nameof(GetInvalidCell));
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, newMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}

		public static void Postfix()
		{
			WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch.GenerateAnywhere = false;
		}
	}
}