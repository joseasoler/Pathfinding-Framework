using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.ErrorHandling
{
	/// <summary>
	/// Store the parameters and result of the last reachability check. Used for error report generation.
	/// </summary>
	public static class LastReachabilityResult
	{
		private static bool _result;
		private static IntVec3 _start;
		private static LocalTargetInfo _dest;
		private static PathEndMode _peMode;
		private static TraverseParms _traverseParams;
		private static MovementDef _movementDef;
		private static bool _avoidFences;
		private static Job _job;

		public static void Store(bool result, IntVec3 start, LocalTargetInfo dest, PathEndMode peMode,
			TraverseParms traverseParams)
		{
			_result = result;
			_start = start;
			_dest = dest;
			_peMode = peMode;
			_traverseParams = traverseParams;
			Pawn pawn = _traverseParams.pawn;
			if (pawn != null)
			{
				_movementDef = pawn.MovementDef();
				_avoidFences = pawn.MovementContext()?.ShouldAvoidFences ?? false;
				_job = pawn.CurJob;
			}
			else
			{
				_movementDef = null;
				_job = null;
			}
		}

		public static string Get()
		{
			Pawn pawn = _traverseParams.pawn;
			Thing thing = _dest.HasThing ? _dest.Thing : null;
			Map map = pawn != null ? pawn.Map : thing?.Map;
			string pawnStr = pawn != null ? pawn.GetUniqueLoadID() : "None";
			string startStr = _start.ToReportString(map);
			string destStr = _dest.Cell.ToReportString(map);
			string targetStr = thing != null ? thing.GetUniqueLoadID() : "Cell";
			string jobStr = _job?.ToString() ?? "None";
			string traverseModeStr = _traverseParams.mode.ToReportString();
			string pathEndMode = _peMode.ToReportString();

			return
				$"Last Reachability.CanReach: {pawnStr} using movement {_movementDef}(avoid fences: {_avoidFences}) to move from {startStr} to {destStr}. Trying to reach {targetStr}. Job: {jobStr}. Traverse: {traverseModeStr}, path end: {pathEndMode}. Result: {_result}";
		}
	}
}