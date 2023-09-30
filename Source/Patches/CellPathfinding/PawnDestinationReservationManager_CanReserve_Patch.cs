using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.CellPathfinding
{
	/// <summary>
	/// Prevent pawns from reserving cells they cannot enter.
	/// </summary>
	[HarmonyPatch(typeof(PawnDestinationReservationManager), nameof(PawnDestinationReservationManager.CanReserve))]
	internal static class PawnDestinationReservationManager_CanReserve_Patch
	{
		internal static void Postfix(ref bool __result, IntVec3 c, Pawn searcher)
		{
			bool original = __result;
			var terrainDef = c.GetTerrain(searcher.Map);
			if (__result && !searcher.MovementContext().CanEnterTerrain(c))
			{
				__result = false;
			}
		}
	}
}