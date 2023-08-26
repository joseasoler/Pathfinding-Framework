using HarmonyLib;
using PathfindingFramework.Debug;
using RimWorld;

namespace PathfindingFramework.Patches.Debug
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