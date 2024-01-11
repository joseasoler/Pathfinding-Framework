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
	public static class Colonist_UpdateOrderPos_Patch
	{
		private const string TypeName = "Colonist";
		private const string MethodName = "UpdateOrderPos";

		public static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.AchtungAssembly != null;
		}

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.AchtungAssembly, TypeName, MethodName);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.Standable));

			MethodInfo modifiedMethod = AccessTools.Method(typeof(LocationFinding), nameof(LocationFinding.CanPawnStandAt));

			FieldInfo pawnFieldInfo =
				ModCompatibilityUtility.FieldFromAssembly(ModAssemblyInfo.AchtungAssembly, TypeName, "pawn");

			if (pawnFieldInfo == null)
			{
				Report.Error($"Could not apply Harmony patch for Achtung compatibility: Colonist.pawn field was not found.");
			}

			bool transpilerApplied = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld, pawnFieldInfo);
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