using HarmonyLib;
using PathfindingFramework.DevTool;
using RimWorld;

namespace PathfindingFramework.Patches.DevTool.DevToolDefGeneration
{
	[HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
	internal static class DefGenerator_GenerateImpliedDefs_PreResolve_Patch
	{
		internal static void Postfix()
		{
			HediffDefGenerator_Movement.GenerateMovementHediffDefs();
		}
	}
}