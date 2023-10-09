using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Log additional data when a pawn fails to generate a valid path.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_PathFollower), "GenerateNewPath")]
	internal static class Pawn_PathFollower_GenerateNewPath
	{
		internal static void Postfix(Pawn ___pawn, LocalTargetInfo ___destination, PawnPath __result)
		{
			if (__result == PawnPath.NotFound && Settings.Values.LogPathNotFound)
			{
				Report.Warning(
					$"Additional pathfinding failure information: {___pawn.GetUniqueLoadID()} ({___pawn.Name}) with movement {___pawn.MovementDef().LabelCap} moving from {___pawn.Position} to {___destination.Cell}");
			}
		}
	}
}