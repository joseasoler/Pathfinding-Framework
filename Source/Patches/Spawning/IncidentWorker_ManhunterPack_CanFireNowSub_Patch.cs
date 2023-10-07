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
	/// Make the manhunter incident check movement type aware.
	/// Without this patch, TryFindManhunterAnimalKind could return an aquatic animal and then TryFindRandomPawnEntryCell
	/// would fail.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_ManhunterPack), "CanFireNowSub")]
	public class IncidentWorker_ManhunterPack_CanFireNowSub_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
					yield return new CodeInstruction(OpCodes.Ldloc_1); // animalKind
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