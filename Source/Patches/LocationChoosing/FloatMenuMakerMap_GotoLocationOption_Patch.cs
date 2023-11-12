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
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo standableCellNearOriginalMethod =
				AccessTools.Method(typeof(CellFinder), nameof(CellFinder.StandableCellNear));

			MethodInfo standableCellNearNewMethod =
				AccessTools.Method(typeof(LocationFinding),
					nameof(LocationFinding.StandableCellNearForMovementType));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo info &&
				    info == standableCellNearOriginalMethod)
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