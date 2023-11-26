using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.GeologicalLandforms
{
	/// <summary>
	/// Make the check aware of pawn movement types.
	/// </summary>
	public static class Patch_RimWorld_CellFinder_TryFindRandomExitSpot_Patch
	{
		private const string TypeName = "Patch_RimWorld_CellFinder";
		private const string MethodName = "TryFindRandomExitSpot";

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GeologicalLandformsAssembly, TypeName,
				MethodName);
		}

		public static void Patch(HarmonyLib.Harmony harmonyInstance)
		{
			MethodBase originalMethod = TargetMethod();
			HarmonyMethod transpilerMethod =
				new HarmonyMethod(AccessTools.Method(typeof(Patch_RimWorld_CellFinder_TryFindRandomExitSpot_Patch),
					nameof(Transpiler)));
			harmonyInstance.Patch(originalMethod, transpiler: transpilerMethod);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalStandableMethod = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.Standable));

			MethodInfo movementStandableMethod =
				AccessTools.Method(typeof(LocationFinding), nameof(LocationFinding.CanPawnStandAt));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalStandableMethod))
				{
					// Replace GenGrid.Standable with a pawn aware version.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // pawn
					yield return new CodeInstruction(OpCodes.Call, movementStandableMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}