using System.Collections.Generic;
using System.Globalization;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.MapPathCosts;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Generates an estimated memory usage report. This is not meant to be entirely accurate; instead it is used to get
	/// a general idea of how the reserved memory amount changes with different implementations, and to detect regressions
	/// and bugs.
	/// </summary>
	public class MemoryUsageDebugOutput
	{
		/// <summary>
		/// Convert a byte numerical amount into a string in kibibytes. 
		/// </summary>
		/// <param name="bytes">Number of bytes</param>
		/// <returns>Kibibyte representation.</returns>
		private static string ToKiB(int bytes)
		{
			return $"{(bytes / 1024.0F).ToString("0.##", CultureInfo.InvariantCulture)} KiB";
		}

		[DebugOutput(category: PathfindingFramework.Name, onlyWhenPlaying: false)]
		public static void MemoryUsageEstimation()
		{
			var reports = new List<MemoryUsageData>();
			reports.AddRange(MovementExtensionCache.MemoryReport());
			reports.AddRange(MovementDefUtils.MemoryReport.Get());
			reports.AddRange(MapPathCostMemoryReport.MemoryReport());
			var dataTable = new string[4, reports.Count + 2];
			dataTable[0, 0] = "Cache";
			dataTable[1, 0] = "Map";
			dataTable[2, 0] = "Grid";
			dataTable[3, 0] = "KiB";

			var totalBytes = 0;
			for (var reportIndex = 0; reportIndex < reports.Count; ++reportIndex)
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