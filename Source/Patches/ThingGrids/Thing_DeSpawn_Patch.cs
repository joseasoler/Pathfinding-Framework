using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ThingGrids
{
	public class DeSpawnThingState
	{
		public CellRect Rect;
		public Map Map;
	}

	/// <summary>
	/// Update path costs of things when a thing de-spawns.
	/// </summary>
	[HarmonyPatch(typeof(Thing), nameof(Thing.DeSpawn))]
	public static class Thing_DeSpawn_Patch
	{
		public static void Prefix(Thing __instance, out DeSpawnThingState __state)
		{
			if (__instance.def.pathCost != 0 || __instance.def.passability == Traversability.Impassable)
			{
				__state = new DeSpawnThingState
				{
					Map = __instance.Map,
					Rect = __instance.OccupiedRect()
				};
			}
			else
			{
				__state = new DeSpawnThingState();
			}
		}

		public static void Postfix(DeSpawnThingState __state)
		{
			if (__state.Map == null)
			{
				return;
			}

			foreach (IntVec3 cell in __state.Rect)
			{
				__state.Map.MapPathCostGrid().UpdateThings(cell);
			}
		}
	}
}