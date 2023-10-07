using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	[HarmonyPatch(typeof(IncidentWorker_ManhunterPack), "TryExecuteWorker")]
	public class IncidentWorker_ManhunterPack_TryExecuteWorker_Patch
	{
		private static Pawn _lastPawn;
		private static Pawn _currentPawn;

		private static IntVec3 AdaptRandomClosewalkCellNear(IntVec3 root, Map __, int radius,
			Predicate<IntVec3> extraValidator)
		{
			// The original code uses CellFinder.RandomClosewalkCellNear, which uses a TraverseParms.
			// For movement aware pathfinding, TraverseParms requires a spawned pawn.
			// Since the pawn is not spawned yet, a workaround must be used.
			IntVec3 result = _lastPawn == null
				// For the first pawn, use the position calculated by LocationFinding.TryFindRandomPawnEntryCell.
				? root
				// Use the last generated pawn to calculate the position of the new one.
				: LocationFinding.RandomClosewalkCellNear(_lastPawn, radius, extraValidator);

			// Store the last pawn for the next execution.
			_lastPawn = _currentPawn;
			return result;
		}

		private static void StoreCurrentPawn(Pawn pawn)
		{
			_currentPawn = pawn;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			IEnumerable<CodeInstruction> newInstructions =
				IncidentWorker_ManhunterPack_Util.Transpile_TryFindRandomPawnEntryCell(OpCodes.Ldloc_1, instructions);

			MethodInfo vanillaClosewalkMethod =
				AccessTools.Method(typeof(CellFinder),
					nameof(CellFinder.RandomClosewalkCellNear));

			MethodInfo pawnClosewalkMethod =
				AccessTools.Method(typeof(IncidentWorker_ManhunterPack_TryExecuteWorker_Patch),
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
						AccessTools.Method(typeof(IncidentWorker_ManhunterPack_TryExecuteWorker_Patch),
							nameof(StoreCurrentPawn));

					yield return new CodeInstruction(OpCodes.Call, objTest);
				}
			}
		}

		internal static void Postfix()
		{
			// Clear state for the next execution.
			_lastPawn = null;
			_currentPawn = null;
		}
	}
}