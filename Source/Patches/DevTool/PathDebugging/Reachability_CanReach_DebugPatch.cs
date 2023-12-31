﻿using HarmonyLib;
using PathfindingFramework.ErrorHandling;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Stores the last result of a reachability check. This information is attached to error reports.
	/// </summary>
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(PathEndMode), typeof(TraverseParms))]
	public static class Reachability_CanReach_DebugPatch
	{
		public static void Postfix(bool __result, IntVec3 start, LocalTargetInfo dest, PathEndMode peMode,
			TraverseParms traverseParams)
		{
			LastReachabilityResult.Store(__result, start, dest, peMode, traverseParams);
		}
	}
}