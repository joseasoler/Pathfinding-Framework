using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ThingGrids
{
	internal class DeSpawnThingState
	{
		public CellRect rect;
		public Map map;
	}

	[HarmonyPatch(typeof(Thing), nameof(Thing.DeSpawn))]
	internal static class Thing_DeSpawn_Patch
	{
		internal static void Prefix(Thing __instance, out DeSpawnThingState __state)
		{
			if (__instance.def.pathCost != 0 || __instance.def.passability == Traversability.Impassable)
			{
				__state = new DeSpawnThingState
				{
					map = __instance.Map,
					rect = __instance.OccupiedRect()
				};
			}
			else
			{
				__state = new DeSpawnThingState();
			}
		}

		internal static void Postfix(DeSpawnThingState __state)
		{
			if (__state.map == null)
			{
				return;
			}

			foreach (var cell in __state.rect)
			{
				__state.map.MapPathCostGrid().UpdateThings(cell);
			}
		}
	}
}