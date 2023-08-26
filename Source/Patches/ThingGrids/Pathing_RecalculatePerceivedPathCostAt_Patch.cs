using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ThingGrids
{
	/// <summary>
	/// Update MapPathCostCache when needed.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.RecalculatePerceivedPathCostAt))]
	public class Pathing_RecalculatePerceivedPathCostAt_Patch
	{
		internal static void Prefix(Pathing __instance, IntVec3 c)
		{
			var map = __instance.Normal?.map;
			if (map != null)
			{
				MapPathCostCache.Get(map.uniqueID)?.UpdateThings(c);
			}
		}
	}
}