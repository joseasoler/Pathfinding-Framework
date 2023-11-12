using HarmonyLib;
using PathfindingFramework.ErrorHandling;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Report region / region link generation failures after game load / map generation.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	public static class Map_FinalizeInit_DebugPatch
	{
		public static void Postfix(Map __instance)
		{
			// Commented out, as it sometimes cause false positives. When this happens, this code will report a failure, but
			// later on the manual divergence check will not detect any issues.
			/*
			if (RegionErrorReport.TryGet(__instance, out string regionErrorReport))
			{
				Report.Error($"Region generation failures detected:\n{regionErrorReport}");
			}
			*/
		}
	}
}