using System;
using System.Text;
using HarmonyLib;
using PathfindingFramework.ErrorHandling;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Report region / region link generation failures after game load / map generation.
	/// </summary>
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	public class Map_FinalizeInit_DebugPatch
	{
		internal static void Postfix(Map __instance)
		{
			if (RegionErrorReport.TryGet(__instance, out string regionErrorReport))
			{
				Report.Error($"Region generation failures detected:\n{regionErrorReport}");
			}
		}
	}
}