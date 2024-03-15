using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Log additional data when a pawn fails to generate a valid path.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_PathFollower), "GenerateNewPath")]
	public static class Pawn_PathFollower_GenerateNewPath
	{
		public static void Postfix(Pawn ___pawn, LocalTargetInfo ___destination, PawnPath __result)
		{
			if (__result == PawnPath.NotFound && Settings.Values.LogPathNotFound)
			{
				Map map = ___pawn.Map;
				IntVec3 start = ___pawn.Position;
				IntVec3 end = ___destination.Cell;
				Report.Warning(
					$"Additional pathfinding failure information: {___pawn.GetUniqueLoadID()} ({___pawn.Name}) with movement type{___pawn.MovementDef().LabelCap}.\n" +
					$"Moving from {start}[{start.GetTerrain(map)}] to {end}[{end.GetTerrain(map)}]");
			}
		}
	}
}