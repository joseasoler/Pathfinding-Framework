using System.Collections.Generic;
using System.Globalization;
using LudeonTK;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapPathCosts;
using PathfindingFramework.Patches;
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

		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: false)]
		public static void MemoryUsageEstimation()
		{
			List<MemoryUsageData> reports = new List<MemoryUsageData>();
			reports.AddRange(MovementDefUtils.MemoryReport.Get());
			reports.AddRange(MapPathCostMemoryReport.MemoryReport());
			foreach (Map map in Find.Maps)
			{
				reports.AddRange(map.
					MovementContextData().MemoryReport());
			}

			string[,] dataTable = new string[4, reports.Count + 2];
			dataTable[0, 0] = "Cache";
			dataTable[1, 0] = "Map";
			dataTable[2, 0] = "Grid";
			dataTable[3, 0] = "KiB";

			int totalBytes = 0;
			for (int reportIndex = 0; reportIndex < reports.Count; ++reportIndex)
			{
				MemoryUsageData report = reports[reportIndex];
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