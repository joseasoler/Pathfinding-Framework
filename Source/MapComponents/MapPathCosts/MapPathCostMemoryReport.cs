using System.Collections.Generic;
using PathfindingFramework.DevTool;
using PathfindingFramework.ExtensionMethodCaches;
using Verse;

namespace PathfindingFramework.MapComponents.MapPathCosts
{
	/// <summary>
	/// Generate a memory usage report of the map path cost grids.
	/// </summary>
	public static class MapPathCostMemoryReport
	{
		public static List<MemoryUsageData> MemoryReport()
		{
			const string cacheName = nameof(MapPathCostMemoryReport);
			List<MemoryUsageData> report = new List<MemoryUsageData>();

			int mapPathCostSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MapPathCost));

			foreach (Map map in Find.Maps)
			{
				MapPathCostGrid mapPathCostGrid = map.MapPathCostGrid();
				if (mapPathCostGrid == null)
				{
					continue;
				}

				string mapName = map.GetUniqueLoadID();

				report.Add(new MemoryUsageData(cacheName, mapName, "Map path cost grid",
					mapPathCostSize * mapPathCostGrid.GridSize));
			}

			return report;
		}
	}
}