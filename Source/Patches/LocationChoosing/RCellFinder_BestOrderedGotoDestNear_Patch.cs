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
	internal static class RCellFinder_BestOrderedGotoDestNear_Patch
	{
		private static bool CanPawnStandAt(IntVec3 c, Map _, Pawn pawn)
		{
			return pawn.MovementContext().CanStandAt(c);
		}

		private static MethodBase TargetMethod()
		{
			foreach (Type nestedType in typeof(RCellFinder).GetNestedTypes(AccessTools.all))
			{
				foreach (MethodInfo method in nestedType.GetMethods(AccessTools.all))
				{
					if (method.Name.Contains(nameof(RCellFinder.BestOrderedGotoDestNear)))
					{
						return method;
					}
				}
			}

			return null;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.Standable));

			MethodInfo modifiedMethod =
				AccessTools.Method(typeof(RCellFinder_BestOrderedGotoDestNear_Patch),
					nameof(CanPawnStandAt));

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