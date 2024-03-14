using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.LocationChoosing
{
	/// <summary>
	/// Allow drafted move into impassable terrain if the movement context of the pawns allows it.
	/// </summary>
	[HarmonyPatch(typeof(Selector), "MassTakeFirstAutoTakeableOptionOrGoto")]
	public static class Selector_MassTakeFirstAutoTakeableOptionOrGoto_Patch
	{
		public static IntVec3 StandableCellNearForMovementTypesDiscardPredicate(IntVec3 cell, Map map, float radius,
			Predicate<IntVec3> _, List<Pawn> selectedPawns)
		{
			return LocationFinding.StandableCellNearForMovementTypes(cell, map, radius, selectedPawns);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo standableCellNearOriginalMethod =
				AccessTools.Method(typeof(CellFinder), nameof(CellFinder.StandableCellNear));

			MethodInfo standableCellNearNewMethod =
				AccessTools.Method(typeof(Selector_MassTakeFirstAutoTakeableOptionOrGoto_Patch),
					nameof(StandableCellNearForMovementTypesDiscardPredicate));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo info &&
				    info == standableCellNearOriginalMethod)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_0); // selectedPawns
					yield return new CodeInstruction(OpCodes.Call, standableCellNearNewMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}