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
		internal static bool Prefix(ref bool __result, IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParams)
		{
			MovementContext context = traverseParams.pawn?.MovementContext();
			// Prevent movement into a terrain type unsafe for the pawn.
			if (context != null && !context.CanEnterTerrain(dest.Cell))
			{
				__result = false;
				return false;
			}

			return true;
		}

		// Code that can be used to debug reachability issues.
		/*
		private static void Postfix(bool __result, IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParams)
		{
			Pawn pawn = traverseParams.pawn;
			IntVec3 desti = dest.Cell;
			if (pawn == null)
			{
				Report.Error(
					$"Reachability.CanReach postfix without pawn: [{start.x}, {start.z}] -> [{desti.x}, {desti.z}] = {__result}");
				return;
			}

			Map map = pawn.Map;
			Report.Error(
				$"Reachability.CanReach postfix: [{start.x}, {start.z}, {start.GetTerrain(map)}] -> [{desti.x}, {desti.z}, {desti.GetTerrain(map)}] = {__result}");
		}
		*/
	}
}