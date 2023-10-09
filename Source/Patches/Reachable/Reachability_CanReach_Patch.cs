using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.MovementContexts;
using PathfindingFramework.Parse;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.Reachable
{
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(PathEndMode), typeof(TraverseParms))]
	internal static class Reachability_CanReach_Patch
	{
		private static bool _disableDistrictCheck = false;

		internal static bool Prefix(ref bool __result, IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParams)
		{
			MovementContext context = traverseParams.pawn?.MovementContext();

			if (context == null)
			{
				_disableDistrictCheck = false;
				return true;
			}

			// Usually, if the start and end of the path are in the same district, the reachability code returns true as it
			// can be sure that all tiles between both cells are traversable.
			// This is not true for aquatic creatures so they must disable this check.
			_disableDistrictCheck = context.MovementDef.defaultCost == PathCost.Avoid;

			// Prevent any movement to a terrain type which which the pawn should avoid.
			if (!context.CanEnterTerrain(dest.Cell))
			{
				__result = false;
				return false;
			}

			return true;
		}

		// Disable same district optimization for aquatic creatures.
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo disableDistrictCheckField =
				AccessTools.Field(typeof(Reachability_CanReach_Patch), "_disableDistrictCheck");
			FieldInfo traverseParmsModeField = AccessTools.Field(typeof(TraverseParms), nameof(TraverseParms.mode));

			List<CodeInstruction> instructionList = instructions.ToList();
			bool match = false;
			for (int index = 0; index < instructionList.Count; ++index)
			{
				CodeInstruction instruction = instructionList[index];
				yield return instruction;

				if (!match && instruction.opcode == OpCodes.Beq_S && instructionList[index - 1].opcode == OpCodes.Ldc_I4_5)
				{
					CodeInstruction fieldInstruction = instructionList[index - 2];
					if (fieldInstruction.opcode == OpCodes.Ldfld && fieldInstruction.operand is FieldInfo operandField &&
					    operandField == traverseParmsModeField)
					{
						match = true;
						yield return new CodeInstruction(OpCodes.Ldsfld, disableDistrictCheckField);
						yield return new CodeInstruction(OpCodes.Brtrue_S, instruction.operand);
					}
				}
			}

			if (!match)
			{
				Report.Error("Reachability_CanReach_Patch: Could not find transpiler insertion point.");
			}
		}

		// Code that can be used to debug reachability issues.
		/*
		private static void Postfix(bool __result, IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParams)
		{
			Pawn pawn = traverseParams.pawn;
			if (pawn == null)
			{
				return;
			}

			Map map = pawn.Map;
			IntVec3 desti = dest.Cell;
			Report.Error(
				$"GlobalReachability: [{start.x}, {start.z}, {start.GetTerrain(map)}] -> [{desti.x}, {desti.z}, {desti.GetTerrain(map)}] = {__result}");
		}
		*/
	}
}