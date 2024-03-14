using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Make the manhunter incident execution aware of different movement types.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_AggressiveAnimals), "TryExecuteWorker")]
	public static class IncidentWorker_AggressiveAnimals_TryExecuteWorker_Patch
	{
		private static Pawn _firstPawn;
		private static Pawn _currentPawn;

		private static IntVec3 AdaptRandomClosewalkCellNear(IntVec3 root, Map __, int radius,
			Predicate<IntVec3> extraValidator)
		{
			// The original code uses CellFinder.RandomClosewalkCellNear, which uses a TraverseParms.
			// For movement aware pathfinding, TraverseParms requires a spawned pawn.
			// Since the pawn is not spawned yet, a workaround must be used.
			IntVec3 result;
			if (_firstPawn == null)
			{
				// For the first pawn, use the position calculated by LocationFinding.TryFindRandomPawnEntryCell.
				result = root;
				// Store the first pawn for the upcoming executions.
				_firstPawn = _currentPawn;
			}
			else
			{
				// Use the last generated pawn to calculate the position of the new one.
				result = LocationFinding.RandomClosewalkCellNear(_firstPawn, radius, extraValidator);
			}

			return result;
		}

		public static void StoreCurrentPawn(Pawn pawn)
		{
			_currentPawn = pawn;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			IEnumerable<CodeInstruction> newInstructions =
				IncidentWorker_ManhunterPack_Util.Transpile_TryFindRandomPawnEntryCell(OpCodes.Ldloc_1, instructions);

			MethodInfo vanillaClosewalkMethod =
				AccessTools.Method(typeof(CellFinder),
					nameof(CellFinder.RandomClosewalkCellNear));

			MethodInfo pawnClosewalkMethod =
				AccessTools.Method(typeof(IncidentWorker_AggressiveAnimals_TryExecuteWorker_Patch),
					nameof(AdaptRandomClosewalkCellNear));

			foreach (CodeInstruction instruction in newInstructions)
			{
				if (instruction.Calls(vanillaClosewalkMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, pawnClosewalkMethod);
				}
				else
				{
					yield return instruction;
				}

				if (instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo operandMethodInfo &&
				    operandMethodInfo.ReturnType == typeof(Pawn))
				{
					yield return new CodeInstruction(OpCodes.Dup); // Duplicate pawn currently on the stack.

					MethodInfo objTest =
						AccessTools.Method(typeof(IncidentWorker_AggressiveAnimals_TryExecuteWorker_Patch),
							nameof(StoreCurrentPawn));

					yield return new CodeInstruction(OpCodes.Call, objTest);
				}
			}
		}

		public static void Postfix()
		{
			// Clear state for the next execution.
			_firstPawn = null;
			_currentPawn = null;
		}
	}
}