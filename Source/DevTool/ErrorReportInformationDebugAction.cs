using PathfindingFramework.ErrorHandling;
using Verse;

namespace PathfindingFramework.DevTool
{
	internal static class ErrorReportInformationDebugAction
	{
		[DebugAction(category: PathfindingFramework.Name, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ErrorReportInformation()
		{
			Report.Error(ErrorReport.Get(Find.CurrentMap));
		}
	}
}