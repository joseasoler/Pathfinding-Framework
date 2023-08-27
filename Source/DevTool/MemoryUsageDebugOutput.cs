using System.Collections.Generic;
using System.Globalization;
using PathfindingFramework.Cache;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.Cache.Local;
using Verse;

namespace PathfindingFramework.DevTool
{
	public class MemoryUsageDebugOutput
	{
		private static string ToKiB(int bytes)
		{
			return $"{(bytes / 1024.0F).ToString("0.##", CultureInfo.InvariantCulture)} KiB";
		}

		[DebugOutput(category: Mod.Name, onlyWhenPlaying: false)]
		public static void MemoryUsageEstimation()
		{
			var reports = new List<MemoryUsageData>();
			reports.AddRange(MovementExtensionCache.MemoryReport());
			reports.AddRange(MovementPathCostCache.MemoryReport());
			reports.AddRange(PawnMovementCache.MemoryReport());
			reports.AddRange(MapPathCostCache.MemoryReport());
			string[,] dataTable = new string[4, reports.Count + 2];
			dataTable[0, 0] = "Cache";
			dataTable[1, 0] = "Map";
			dataTable[2, 0] = "Grid";
			dataTable[3, 0] = "KiB";

			int totalBytes = 0;
			for (int reportIndex = 0; reportIndex < reports.Count; ++reportIndex)
			{
				var report = reports[reportIndex];
				dataTable[0, reportIndex + 1] = report.Cache;
				dataTable[1, reportIndex + 1] = report.Map;
				dataTable[2, reportIndex + 1] = report.Grid;
				dataTable[3, reportIndex + 1] = ToKiB(report.Bytes);
				totalBytes += report.Bytes;
			}

			dataTable[0, reports.Count + 1] = "Total";
			dataTable[1, reports.Count + 1] = MemoryUsageData.Global;
			dataTable[2, reports.Count + 1] = "All";
			dataTable[3, reports.Count + 1] = ToKiB(totalBytes);

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}