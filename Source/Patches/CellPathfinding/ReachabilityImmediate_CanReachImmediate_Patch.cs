using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.CellPathfinding
{
	/// <summary>
	/// Make pawns avoid trying to reach cells they should not be in.
	/// </summary>
	[HarmonyPatch(typeof(ReachabilityImmediate), nameof(ReachabilityImmediate.CanReachImmediate), typeof(IntVec3),
		typeof(LocalTargetInfo), typeof(Map),
		typeof(PathEndMode), typeof(Pawn))]
	internal static class ReachabilityImmediate_CanReachImmediate_Patch
	{
		internal static bool Prefix(ref bool __result, IntVec3 start, LocalTargetInfo target, PathEndMode peMode, Pawn pawn)
		{
			if (target.IsValid && pawn?.MovementContext() != null && !pawn.MovementContext().CanEnterTerrain(target.Cell))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}