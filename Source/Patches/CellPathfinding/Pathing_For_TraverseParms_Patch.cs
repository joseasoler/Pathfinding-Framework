using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.CellPathfinding
{
	/// <summary>
	/// When possible, inject the correct pathing context for each pawn.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.For), typeof(TraverseParms))]
	public static class Pathing_For_TraverseParms_Patch
	{
		public static bool Prefix(ref PathingContext __result, TraverseParms parms)
		{
			PathingContext pathingContext = parms.pawn?.MovementContext()?.PathingContext;
			if (pathingContext != null)
			{
				__result = pathingContext;
			}

			// Allow vanilla execution only if no pathing context was found.
			return pathingContext == null;
		}
	}
}