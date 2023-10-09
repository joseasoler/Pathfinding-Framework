using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// When the animal spawning code chooses an incompatible terrain and animal type, switch to a new target cell.
	/// </summary>
	[HarmonyPatch(typeof(WildAnimalSpawner), nameof(WildAnimalSpawner.SpawnRandomWildAnimalAt))]
	internal static class WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch
	{
		private static bool TryReplaceAnimalSpawnLocation(PawnKindDef pawnKindDef, Map map, IntVec3 location,
			int randomInRange, int radius)
		{
			MovementDef movementDef = MovementExtensionCache.GetMovementDef(pawnKindDef);
			if (randomInRange <= 0 || movementDef == null || !location.InBounds(map))
			{
				return false;
			}

			TerrainDef terrainDef = location.GetTerrain(map);

			if (movementDef.PathCosts[terrainDef.index] < PathCost.Avoid.cost)
			{
				return false;
			}

			// The animal to spawn cannot stand in the chosen terrain. A new cell must be found.
			if (!LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 newCell, map, CellFinder.EdgeRoadChance_Animal, true,
				    null, movementDef))
			{
				return false;
			}

			// Generate the first pawn in the chosen tile.
			Pawn pawn = GenSpawn.Spawn(PawnGenerator.GeneratePawn(pawnKindDef), newCell, map) as Pawn;
			for (int index = 1; index < randomInRange; ++index)
			{
				// Use the first pawn to calculate close valid locations.
				LocationFinding.TryRandomClosewalkCellNear(pawn, radius, out IntVec3 closeLocation);
				GenSpawn.Spawn(PawnGenerator.GeneratePawn(pawnKindDef), closeLocation, map);
			}

			return true;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
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
	}
}