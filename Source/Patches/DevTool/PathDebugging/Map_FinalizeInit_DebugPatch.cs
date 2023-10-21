using System;
using System.Text;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Report link generation failures. Commented out for performance reasons.
	/// </summary>
	/*
	[HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
	public class Map_FinalizeInit_DebugPatch
	{
		internal static void Postfix(Map __instance)
		{
			bool errors = false;
			StringBuilder sb = new StringBuilder();

			foreach (Region region in __instance.regionGrid.AllRegions_NoRebuild_InvalidAllowed)
			{
				foreach (RegionLink link in region.links)
				{
					if (link.GetOtherRegion(region) == null)
					{
						errors = true;
						sb.AppendLine(
							$"Error link in region {region.id}: Root: [{link.span.root.x}, {link.span.root.z}], Dir: {Enum.GetName(typeof(SpanDirection), link.span.dir)}, Len: {link.span.length.ToString()}");
					}
				}
			}

			if (errors)
			{
				Report.Error(sb.ToString());
			}
		}
	}
	*/
}