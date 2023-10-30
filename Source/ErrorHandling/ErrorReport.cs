using System.Text;
using Verse;

namespace PathfindingFramework.ErrorHandling
{
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