using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.FirePathCost
{
	/// <summary>
	/// Keeps the fire grid updated after a new fire spawns.
	/// </summary>
	[HarmonyPatch(typeof(Fire), nameof(Fire.SpawnSetup))]
	public static class Fire_SpawnSetup_Patch
	{
		public static void Postfix(Fire __instance)
		{
			if (__instance.parent is Pawn)
			{
				// Ignore fires attached to pawns in pathfinding calculations.
				return;
			}

			__instance.Map?.MapPathCostGrid().UpdateFire(__instance.Position, true);
		}
	}
}