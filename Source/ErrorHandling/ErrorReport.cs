using System.Text;
using Verse;

namespace PathfindingFramework.ErrorHandling
{
	/// <summary>
	/// Generate an error report including region / region linking errors (if any) and the last reachability check
	/// performed by the game.
	/// </summary>
	public static class ErrorReport
	{
		public static string Get(Map map)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(LastReachabilityResult.Get());
			if (map != null && RegionErrorReport.TryGet(map, out string regionErrorReport))
			{
				sb.Append(regionErrorReport);
			}

			return sb.ToString();
		}
	}
}