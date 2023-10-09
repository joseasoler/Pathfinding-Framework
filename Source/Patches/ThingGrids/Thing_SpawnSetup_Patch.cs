﻿using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ThingGrids
{
	/// <summary>
	/// Update path costs of things when a thing spawns.
	/// </summary>
	[HarmonyPatch(typeof(Thing), nameof(Thing.SpawnSetup))]
	internal static class Thing_SpawnSetup_Patch
	{
		internal static void Postfix(Thing __instance)
		{
			if (__instance.def.pathCost != 0 || __instance.def.passability == Traversability.Impassable)
			{
				if (__instance.def.size == IntVec2.One)
				{
					__instance.Map.MapPathCostGrid().UpdateThings(__instance.Position);
				}
				else
				{
					foreach (var cell in __instance.OccupiedRect())
					{
						__instance.Map.MapPathCostGrid().UpdateThings(cell);
					}
				}
			}
		}
	}
}