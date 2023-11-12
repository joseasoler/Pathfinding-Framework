using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.CellPathfinding
{
	/// <summary>
	/// Prevent pawns from reserving cells they cannot enter.
	/// </summary>
	[HarmonyPatch(typeof(PawnDestinationReservationManager), nameof(PawnDestinationReservationManager.CanReserve))]
	public static class PawnDestinationReservationManager_CanReserve_Patch
	{
		public static void Postfix(ref bool __result, IntVec3 c, Pawn searcher)
		{
			if (!c.InBounds(searcher.Map))
			{
				return;
			}

			if (__result && !searcher.MovementContext().CanEnterTerrain(c))
			{
				__result = false;
			}
		}
	}
}