using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.Pathfinding
{
	/// <summary>
	/// Make pawns avoid cells they should not be in.
	/// </summary>
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(PathEndMode), typeof(TraverseParms))]
	internal static class Reachability_CanReach_Patch
	{
		internal static void Postfix(ref bool __result, Map ___map, IntVec3 start, LocalTargetInfo dest, PathEndMode peMode,
			TraverseParms traverseParams)
		{
			if (__result && traverseParams.pawn != null && !traverseParams.pawn.MovementContext().CanEnter(dest.Cell))
			{
				__result = false;
			}
		}
	}
}