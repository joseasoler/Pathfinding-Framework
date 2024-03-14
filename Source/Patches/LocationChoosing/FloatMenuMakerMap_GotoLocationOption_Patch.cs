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
	/// Allow drafted move into impassable terrain if the movement context of the pawn allows it.
	/// </summary>
	[HarmonyPatch(typeof(FloatMenuMakerMap), "GotoLocationOption")]
	public static class FloatMenuMakerMap_GotoLocationOption_Patch
	{
		public static IntVec3 StandableCellNearForMovementType(IntVec3 cell, Map map, float radius, Predicate<IntVec3> _,
			Pawn pawn)
		{
			return LocationFinding.StandableCellNearForMovementTypes(cell, map, radius, new List<Pawn> {pawn});
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo standableCellNearOriginalMethod =
				AccessTools.Method(typeof(CellFinder), nameof(CellFinder.StandableCellNear));

			MethodInfo standableCellNearNewMethod =
				AccessTools.Method(typeof(FloatMenuMakerMap_GotoLocationOption_Patch),
					nameof(StandableCellNearForMovementType));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(standableCellNearOriginalMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1); // pawn
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