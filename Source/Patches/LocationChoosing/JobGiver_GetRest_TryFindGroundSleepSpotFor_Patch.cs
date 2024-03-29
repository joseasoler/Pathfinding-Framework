using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.LocationChoosing
{
	[HarmonyPatch(typeof(JobGiver_GetRest), "TryFindGroundSleepSpotFor")]
	public static class JobGiver_GetRest_TryFindGroundSleepSpotFor_Patch
	{
		/// <summary>
		/// For some unknown reason, injecting an extra pawn argument during the transpiling always fails.
		/// This workaround is being used instead.
		/// </summary>
		private static Pawn _pawn;

		public static bool ModifiedTryRandomClosewalkCellNear(IntVec3 _, Map __, int radius, out IntVec3 result,
			Predicate<IntVec3> ___)
		{
			return LocationFinding.TryRandomClosewalkCellNear(_pawn, radius, out result,
				cell => !cell.IsForbidden(_pawn) && !_pawn.MovementContext().AvoidWanderAt(cell));
		}

		public static void Prefix(Pawn pawn)
		{
			_pawn = pawn;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalClosewalkMethod =
				AccessTools.Method(typeof(CellFinder), nameof(CellFinder.TryRandomClosewalkCellNear));

			MethodInfo modifiedClosewalkMethod =
				AccessTools.Method(typeof(JobGiver_GetRest_TryFindGroundSleepSpotFor_Patch),
					nameof(ModifiedTryRandomClosewalkCellNear));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalClosewalkMethod))
				{
					CodeInstruction newCall = new CodeInstruction(OpCodes.Call, modifiedClosewalkMethod);
					instruction.MoveLabelsTo(newCall);
					yield return newCall;
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}