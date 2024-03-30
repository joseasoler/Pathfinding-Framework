using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.MapComponents.MovementContexts;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.LocationChoosing
{
	/// <summary>
	/// Make the reachability check of this function pawn aware.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
	public static class Pawn_PathFollower_StartPath_Patch
	{
		private static Pawn _pawn;

		public static TraverseParms ModifiedParmsFor(TraverseMode mode, Danger maxDanger, bool canBashDoors,
			bool alwaysUseAvoidGrid, bool canBashFences)
		{
			TraverseParms result = TraverseParms.For(_pawn, maxDanger, mode, canBashDoors, alwaysUseAvoidGrid, canBashFences);
			// The TraverseParms.For overload used above does not take into account the current pawn job to set fenceBlocked.
			// Set it correctly here instead.
			result.fenceBlocked = MovementContextUtil.ShouldAvoidFences(_pawn);
			return result;
		}

		public static void Prefix(Pawn ___pawn)
		{
			_pawn = ___pawn;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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