using System.Collections.Generic;
using PathfindingFramework.DevTool;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MapPathCosts
{
	/// <summary>
	/// Stores and keeps updated different pathfinding grids for a specific map.
	/// </summary>
	public static class MapPathCostMemoryReport
	{
		public static List<MemoryUsageData> MemoryReport()
		{
			var cacheName = nameof(MapPathCostMemoryReport);
			var report = new List<MemoryUsageData>();

			int mapPathCostSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MapPathCost));

			foreach (Map map in Find.Maps)
			{
				var mapPathCostGrid = map.MapPathCostGrid();
				if (mapPathCostGrid == null)
				{
					continue;
				}

				var mapName = map.GetUniqueLoadID();

				report.Add(new MemoryUsageData(cacheName, mapName, "Map path cost grid",
					mapPathCostSize * mapPathCostGrid.GridSize));
			}

			return report;
		}
	}
}