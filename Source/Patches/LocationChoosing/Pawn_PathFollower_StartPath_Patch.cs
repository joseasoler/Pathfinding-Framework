using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.LocationChoosing
{
	[HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
	internal static class Pawn_PathFollower_StartPath_Patch
	{
		private static Pawn _pawn;

		private static TraverseParms ModifiedParmsFor(TraverseMode mode, Danger maxDanger, bool canBashDoors,
			bool alwaysUseAvoidGrid, bool canBashFences)
		{
			return TraverseParms.For(_pawn, maxDanger, mode, canBashDoors, alwaysUseAvoidGrid, canBashFences);
		}

		private static void Prefix(Pawn ___pawn)
		{
			_pawn = ___pawn;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod =
				AccessTools.Method(typeof(TraverseParms), nameof(TraverseParms.For),
					new[] { typeof(TraverseMode), typeof(Danger), typeof(bool), typeof(bool), typeof(bool) });

			MethodInfo modifiedMethod =
				AccessTools.Method(typeof(Pawn_PathFollower_StartPath_Patch),
					nameof(ModifiedParmsFor));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, modifiedMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}