using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.Achtung
{
	/// <summary>
	/// Allow drafted move into impassable terrain if the movement context of the pawn allows it.
	/// This requires replacing the vanilla Standable call used by Achtung with a movement type aware version.
	/// </summary>
	[HarmonyPatch]
	public static class Controller_MouseDown_Patch
	{
		private const string TypeName = "Controller";
		private const string MethodName = "MouseDown";

		public static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.AchtungAssembly != null;
		}

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.AchtungAssembly, TypeName, MethodName);
		}

		private static FieldInfo ColonistPawnField;

		public static bool StandableForColonistsList(IntVec3 cell, Map map, List<object> colonists)
		{
			List<Pawn> pawns = new List<Pawn>();
			foreach (object colonist in colonists)
			{
				pawns.Add(ColonistPawnField.GetValue(colonist) as Pawn);
			}

			return pawns.Count != 0 &&
			       LocationFinding.StandableCellNearForMovementTypes(cell, map, 1, pawns).IsValid;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			// Cache a field info used to obtain the pawn from Acthung's Colonist class.
			ColonistPawnField =
				ModCompatibilityUtility.FieldFromAssembly(ModAssemblyInfo.AchtungAssembly, "Colonist", "pawn");

			// Transpile the target method.
			MethodInfo originalMethod = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.Standable));
			MethodInfo modifiedMethod =
				AccessTools.Method(typeof(Controller_MouseDown_Patch), nameof(StandableForColonistsList));

			FieldInfo colonistsFieldInfo =
				ModCompatibilityUtility.FieldFromAssembly(ModAssemblyInfo.AchtungAssembly, TypeName, "colonists");

			if (colonistsFieldInfo == null)
			{
				Report.Error(
					$"Could not apply Harmony patch for Achtung compatibility: Controller.colonists field was not found.");
			}

			bool transpilerApplied = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld, colonistsFieldInfo);
					yield return new CodeInstruction(OpCodes.Call, modifiedMethod);
					transpilerApplied = true;
				}
				else
				{
					yield return instruction;
				}
			}

			if (!transpilerApplied)
			{
				Report.Error($"Could not apply Harmony patch for Achtung compatibility: GenGrid.Standable replacement failed.");
			}
		}
	}
}