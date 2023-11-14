using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.LocationChoosing
{
	/// <summary>
	/// Make the check aware of pawn movement types.
	/// </summary>
	[HarmonyPatch(typeof(RCellFinder), nameof(RCellFinder.TryFindRandomExitSpot))]
	public static class RCellFinder_TryFindRandomExitSpot_Patch
	{
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
					yield return new CodeInstruction(OpCodes.Ldarg_0); // pawn
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