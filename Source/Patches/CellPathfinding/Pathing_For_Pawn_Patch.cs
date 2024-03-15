using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.CellPathfinding
{
	/// <summary>
	/// Inject the correct pathing context for each pawn.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.For), typeof(Pawn))]
	public static class Pathing_For_Pawn_Patch
	{
		public static bool Prefix(ref PathingContext __result, Pawn pawn)
		{
			// Surprisingly, pawns can be null during game initialization, so null conditional access is required.
			PathingContext pathingContext = pawn?.MovementContext()?.PathingContext;
			if (pathingContext != null)
			{
				__result = pathingContext;
			}

			// Allow vanilla execution only if no pathing context was found.
			return pathingContext == null;
		}
	}
}