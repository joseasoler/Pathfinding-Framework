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
		internal static bool Prefix(ref bool __result, IntVec3 c, Pawn searcher, bool draftedOnly)
		{
			if (!searcher.MovementContext().CanEnterTerrain(c))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}