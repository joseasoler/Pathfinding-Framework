using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.Patches.ThingGrids
{
	/// <summary>
	/// Update MapPathCostCache when needed.
	/// </summary>
	[HarmonyPatch(typeof(Thing), nameof(Thing.DeSpawn))]
	public class Thing_DeSpawn_Patch
	{
		internal static void Prefix(Thing __instance)
		{
			if (__instance.def.pathCost != 0 || __instance.def.passability == Traversability.Impassable)
			{
				MapPathCostCache.Get(__instance.Map.uniqueID).UpdateThings(__instance.Position);
			}
		}
	}
}