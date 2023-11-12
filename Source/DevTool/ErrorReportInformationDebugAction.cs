using PathfindingFramework.ErrorHandling;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Show region / region linking errors and the last reachability check performed by the game.
	/// </summary>
	public static class ErrorReportInformationDebugAction
	{
		[DebugAction(category: PathfindingFrameworkMod.Name, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void ErrorReportInformation()
		{
			Report.Error(ErrorReport.Get(Find.CurrentMap));
		}
	}
}