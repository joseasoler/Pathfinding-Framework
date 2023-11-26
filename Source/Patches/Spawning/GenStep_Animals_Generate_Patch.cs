using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// To ensure mod compatibility, it is not possible to replace GenStep_Animals with a custom GenStep, as some mods
	/// such as Geological Landforms target this with prefixes. Other mods might also use this GenStep on alternate map
	/// generation XML files and it would be difficult to catch all of them through XML patching.
	///
	/// RandomAnimalSpawnCell_MapGen only allows districts that touch the map border. With the region generation changes
	/// of Pathfinding Framework, this means that on islands only shallow water districts would be allowed. Since all of
	/// that terrain is allowWander, the function always fails and thus never finds valid places for land creatures. The
	/// patch below replaces this call with an alternate one which ignores avoidWander and districts being on the border.
	///
	/// This cell can later be improved upon if WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch finds out that the chosen
	/// animal cannot stand on it. Then a new walkable cell will be chosen.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_Animals), nameof(GenStep_Animals.Generate))]
	public static class GenStep_Animals_Generate_Patch
	{
		public static IntVec3 Reduced_RandomAnimalSpawnCell_MapGen(Map map)
		{
			if (!CellFinderLoose.TryGetRandomCellWith(cell => cell.Standable(map) && !cell.GetTerrain(map).avoidWander, map,
				    1000, out var result))
			{
				result = CellFinder.RandomCell(map);
			}

			return result;
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
				AccessTools.Method(typeof(GenStep_Animals_Generate_Patch), nameof(Reduced_RandomAnimalSpawnCell_MapGen));
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