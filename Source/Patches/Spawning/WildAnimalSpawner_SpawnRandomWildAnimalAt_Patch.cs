using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Jobs;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// When the animal spawning code chooses an incompatible terrain and animal type, switch to a new target cell.
	/// Prior to this call, Geological Landforms / Biome Transitions ensures that the animal is on a cell that belongs to
	/// its biome of origin. Although this patch does not respect that limitation, the cases in which this is problematic
	/// should be minimal.
	/// </summary>
	[HarmonyPatch(typeof(WildAnimalSpawner), nameof(WildAnimalSpawner.SpawnRandomWildAnimalAt))]
	public static class WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch
	{
		/// <summary>
		/// By default this patch only generates border positions. If this value is set to true (such as for example during
		/// map generation) animals can instead generate on any place from where they can reach the border.
		/// </summary>
		public static bool GenerateAnywhere = false;

		private static List<Pawn> _animalList;

		private static bool TryFindSpawnCell(Map map, PawnKindDef pawnKindDef, out IntVec3 chosenCell)
		{
			if (!GenerateAnywhere)
			{
				return LocationFinding.TryFindRandomPawnEntryCell(out chosenCell, map, CellFinder.EdgeRoadChance_Animal,
					true, null, pawnKindDef);
			}

			MovementDef movementDef = pawnKindDef.race?.MovementDef() ?? MovementDefOf.PF_Movement_Terrestrial;

			bool result = CellFinderLoose.TryGetRandomCellWith(testCell =>
					LocationFinding.CanStandAt(movementDef, map, testCell) &&
					LocationFinding.CanReachMapEdge(movementDef, map, testCell),
				map, 1000, out chosenCell);

			return result;
		}

		public static bool TryReplaceAnimalSpawnLocation(PawnKindDef pawnKindDef, Map map, IntVec3 location,
			int randomInRange, int radius)
		{
			MovementDef movementDef = PawnMovementOverrideSettings.CurrentMovementDef(pawnKindDef.race);
			if (randomInRange <= 0 || movementDef == null)
			{
				return false;
			}

			TerrainDef terrainDef = location.GetTerrain(map);
			if (terrainDef != null &&
			    movementDef.PathCosts[terrainDef.MovementIndex()] < PathCost.Unsafe.cost &&
			    LocationFinding.CanReachMapEdge(movementDef, map, location))
			{
				// If the animal is on safe terrain and can reach the map edge, this is a good position for it to spawn.
				return false;
			}

			// The animal to spawn cannot stand in the chosen terrain. A new cell must be found.
			if (!TryFindSpawnCell(map, pawnKindDef, out IntVec3 newCell))
			{
				return false;
			}

			// Generate the first pawn in the chosen tile.
			Pawn pawn = GenSpawn.Spawn(PawnGenerator.GeneratePawn(pawnKindDef), newCell, map) as Pawn;
			_animalList.Add(pawn);
			for (int index = 1; index < randomInRange; ++index)
			{
				// Use the first pawn to calculate close valid locations.
				LocationFinding.TryRandomClosewalkCellNear(pawn, radius, out IntVec3 closeLocation);
				_animalList.Add((Pawn) GenSpawn.Spawn(PawnGenerator.GeneratePawn(pawnKindDef), closeLocation, map));
			}

			return true;
		}

		private static void StoreAnimal(Pawn pawn)
		{
			_animalList.Add(pawn);
		}

		private static void Prefix()
		{
			_animalList = new List<Pawn>();
		}

		private static IEnumerable<CodeInstruction> ReplaceAnimalSpawnLocations(IEnumerable<CodeInstruction> instructions,
			ILGenerator generator)
		{
			MethodInfo tryReplaceAnimalSpawnLocationMethod =
				AccessTools.Method(typeof(WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch),
					nameof(TryReplaceAnimalSpawnLocation));

			FieldInfo mapField = AccessTools.Field(typeof(WildAnimalSpawner), "map");

			Label label = generator.DefineLabel();
			bool insertLabel = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (insertLabel)
				{
					instruction.labels.Add(label);
					insertLabel = false;
				}

				yield return instruction;
				if (instruction.opcode == OpCodes.Stloc_2)
				{
					// After the random animal, randomInRange and Radius have been calculated and stored, insert a call to our
					// code for checking if the location requires replacement for this animal.
					yield return new CodeInstruction(OpCodes.Ldloc_0); // result (PawnKindDef)
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld, mapField); // RimWorld.WildAnimalSpawner::map
					yield return new CodeInstruction(OpCodes.Ldarg_1); // loc
					yield return new CodeInstruction(OpCodes.Ldloc_1); // randomInRange
					yield return new CodeInstruction(OpCodes.Ldloc_2); // radius
					yield return new CodeInstruction(OpCodes.Call, tryReplaceAnimalSpawnLocationMethod);
					// If tryReplaceAnimalSpawnLocationMethod returned false, call the vanilla code.
					yield return new CodeInstruction(OpCodes.Brfalse_S, label);
					// Return true unconditionally.
					yield return new CodeInstruction(OpCodes.Ldc_I4_1);
					yield return new CodeInstruction(OpCodes.Ret);
					// The next instruction must add the chosen label.
					insertLabel = true;
				}
			}
		}

		private static IEnumerable<CodeInstruction> CollectAnimalList(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo storeAnimalMethod =
				AccessTools.Method(typeof(WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch), nameof(StoreAnimal));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Pop)
				{
					yield return new CodeInstruction(OpCodes.Call, storeAnimalMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator generator)
		{
			return CollectAnimalList(ReplaceAnimalSpawnLocations(instructions, generator));
		}

		private static void Postfix()
		{
			AnimalRelocationUtil.HandleGroup(_animalList);
		}
	}
}