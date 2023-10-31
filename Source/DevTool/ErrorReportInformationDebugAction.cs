using PathfindingFramework.ErrorHandling;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Show region / region linking errors and the last reachability check performed by the game.
	/// </summary>
	internal static class ErrorReportInformationDebugAction
	{
		[DebugAction(category: PathfindingFrameworkMod.Name, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ErrorReportInformation()
		{
			Report.Error(ErrorReport.Get(Find.CurrentMap));
		}
	}
}