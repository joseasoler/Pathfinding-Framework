﻿using HarmonyLib;
using PathfindingFramework.DevTool;
using RimWorld;

namespace PathfindingFramework.Patches.DevTool.DevToolDefGeneration
{
	/// <summary>
	/// Create a HediffDef for each MovementDef. They can be used to arbitrarily assign movement types for testing.
	/// </summary>
	[HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
	public static class DefGenerator_GenerateImpliedDefs_PreResolve_Patch
	{
		public static void Postfix()
		{
			HediffDefGenerator_Movement.GenerateMovementHediffDefs();
		}
	}
}