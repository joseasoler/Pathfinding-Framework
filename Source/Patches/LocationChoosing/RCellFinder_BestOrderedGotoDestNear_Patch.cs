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
	[HarmonyPatch]
	public static class RCellFinder_BestOrderedGotoDestNear_Patch
	{
		public static MethodBase TargetMethod()
		{
			foreach (MethodInfo method in AccessTools.GetDeclaredMethods(typeof(RCellFinder)))
			{
				if (method.Name.Contains("IsGoodDest"))
				{
					return method;
				}
			}

			return null;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.Standable));

			MethodInfo modifiedMethod = AccessTools.Method(typeof(LocationFinding), nameof(LocationFinding.CanPawnStandAt));

			FieldInfo searcherFieldInfo = null;

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld, searcherFieldInfo); // searcher
					yield return new CodeInstruction(OpCodes.Call, modifiedMethod);
				}
				else
				{
					if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo &&
					    fieldInfo.FieldType == typeof(Pawn))
					{
						searcherFieldInfo = fieldInfo;
					}

					yield return instruction;
				}
			}
		}
	}
}