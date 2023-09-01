using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.SnowPathCost
{
	/// <summary>
	/// Keeps pathfinding information updated after snow level changes.
	/// </summary>
	[HarmonyPatch(typeof(SnowGrid), "CheckVisualOrPathCostChange")]
	internal static class SnowGrid_CheckVisualOrPathCostChange_Patch
	{
		internal static void Postfix(SnowGrid __instance, IntVec3 c, float oldDepth, float newDepth)
		{
			SnowCategory newCategory = SnowUtility.GetSnowCategory(newDepth);
			if (SnowUtility.GetSnowCategory(oldDepth) == newCategory)
			{
				return;
			}

			Map map = __instance.map;
			map.MapPathCostGrid().UpdateSnow(c, SnowUtility.MovementTicksAddOn(map.snowGrid.GetCategory(c)));
		}
	}
}