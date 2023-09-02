using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.Pathfinding
{
	/*
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.For), typeof(TraverseParms))]
	internal static class Pathing_For_TraverseParms_Patch
	{
		internal static bool Prefix(ref PathingContext __result, TraverseParms parms)
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
	*/
}