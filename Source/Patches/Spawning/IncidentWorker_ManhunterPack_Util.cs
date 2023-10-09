using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Cache.Global;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Transpiler utilities used by the manhunter incident.
	/// </summary>
	public static class IncidentWorker_ManhunterPack_Util
	{
		public static IEnumerable<CodeInstruction> Transpile_TryFindRandomPawnEntryCell(OpCode pawnKindDefInstruction,
			IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo vanillaEntryCellMethod =
				AccessTools.Method(typeof(RCellFinder),
					nameof(RCellFinder.TryFindRandomPawnEntryCell));

			MethodInfo getMovementDefMethod =
				AccessTools.Method(typeof(MovementExtensionCache),
					nameof(MovementExtensionCache.GetMovementDef), new Type[] {typeof(PawnKindDef)});

			MethodInfo movementEntryCellMethod =
				AccessTools.Method(typeof(LocationFinding),
					nameof(LocationFinding.TryFindRandomPawnEntryCell));


			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(vanillaEntryCellMethod))
				{
					yield return new CodeInstruction(pawnKindDefInstruction); // animal kind pawnKindDef.
					yield return new CodeInstruction(OpCodes.Call, getMovementDefMethod);
					yield return new CodeInstruction(OpCodes.Call, movementEntryCellMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}