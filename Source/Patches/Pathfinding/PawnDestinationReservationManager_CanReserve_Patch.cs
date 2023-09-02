using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.Pathfinding
{
	/// <summary>
	/// Prevent pawns from reserving cells they cannot enter.
	/// </summary>
	[HarmonyPatch(typeof(PawnDestinationReservationManager), nameof(PawnDestinationReservationManager.CanReserve))]
	internal static class PawnDestinationReservationManager_CanReserve_Patch
	{
		internal static bool Prefix(ref bool __result, IntVec3 c, Pawn searcher, bool draftedOnly)
		{
			if (!searcher.MovementContext().CanEnter(c))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}