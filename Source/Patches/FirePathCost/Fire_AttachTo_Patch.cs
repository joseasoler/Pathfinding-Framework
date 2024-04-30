using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.FirePathCost
{
	[HarmonyPatch(typeof(Fire), nameof(Fire.AttachTo))]
	public static class Fire_AttachTo_Patch
	{
		public static void Prefix(Fire __instance, Thing newParent)
		{
			// Since fires attached to pawns are ignored in pathfinding calculations for PF, remove the fire from the grid.
			if (newParent is Pawn)
			{
				__instance.Map?.MapPathCostGrid().UpdateFire(__instance.Position, false);
			}
		}
	}
}