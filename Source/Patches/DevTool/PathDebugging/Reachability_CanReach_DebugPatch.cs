using System;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/*
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(PathEndMode), typeof(TraverseParms))]
	internal static class Reachability_CanReach_DebugPatch
	{
		// Code for debugging reachability issues. WARNING: VERY spammy.
		private static void Postfix(bool __result, IntVec3 start, LocalTargetInfo dest, PathEndMode peMode,
			TraverseParms traverseParams)
		{
			Pawn pawn = traverseParams.pawn;
			Thing thing = dest.HasThing ? dest.Thing : null;
			Map map = pawn != null ? pawn.Map : thing?.Map;
			string pawnStr = pawn != null ? pawn.GetUniqueLoadID() : "None";
			string startStr = $"[{start.x}, {start.z}]";
			string startTerrain = map != null && start.InBounds(map) ? start.GetTerrain(map).defName : "Unknown";
			string destStr = $"[{dest.Cell.x}, {dest.Cell.z}]";
			string destTerrain = map != null && dest.Cell.InBounds(map) ? dest.Cell.GetTerrain(map).defName : "Unknown";
			string targetStr = thing != null ? thing.GetUniqueLoadID() : "Cell";
			string traverseModeStr = Enum.GetName(typeof(TraverseMode), traverseParams.mode);
			string pathEndMode = Enum.GetName(typeof(PathEndMode), peMode);

			Report.Warning(
				$"{pawnStr} requests a Reachability.CanReach from {startStr}[{startTerrain}] to {destStr}[{destTerrain}] to reach {targetStr}. Using traverse mode {traverseModeStr} and path end mode {pathEndMode}. Result is {__result}");
		}
	}
	*/
}