using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.Cache.Local
{
	/// <summary>
	/// Stores and keeps updated different pathfinding grids for a specific map.
	/// </summary>
	public static class MapPathCostCache
	{
		/// <summary>
		/// Associates a map path cost cache to each map.uniqueID value.
		/// </summary>
		private static readonly Dictionary<int, MapPathCosts> MapPathCostsByMap = new();

		/// <summary>
		/// Obtain the map path cost cache of a specific map.
		/// </summary>
		/// <param name="map">Map being requested</param>
		/// <returns>Map path cost cache, or null if the map is being initialized or destroyed.</returns>
		public static MapPathCosts MapPathCosts(this Map map)
		{
			return MapPathCostsByMap.TryGetValue(map.uniqueID, out var result) ? result : null;
		}

		/// <summary>
		/// Clear all map path cost caches.
		/// </summary>
		public static void Clear()
		{
			MapPathCostsByMap.Clear();
		}

		/// <summary>
		/// Start accepting pathfinding update calls after the map is fully initialized.
		/// </summary>
		public static void Add(Map map)
		{
			if (map.uniqueID < 0)
			{
				// m00nl1ght.MapPreview uses maps without uniqueID to generate previews.
				return;
			}

			MapPathCostsByMap.Add(map.uniqueID, new MapPathCosts(map));
		}

		/// <summary>
		/// Clean up the global map cache and stop accepting pathfinding update calls.
		/// </summary>
		public static void Remove(Map map)
		{
			MapPathCostsByMap.Remove(map.uniqueID);
		}

		public static List<MemoryUsageData> MemoryReport()
		{
			var cacheName = nameof(MapPathCostCache);
			var report = new List<MemoryUsageData>();

			int mapPathCostSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MapPathCost));

			foreach (var mapCache in MapPathCostsByMap)
			{
				var mapPathCosts = mapCache.Value;
				var mapName = mapPathCosts.MapName();

				report.Add(new MemoryUsageData(cacheName, mapName, "Map path cost grid",
					mapPathCostSize * mapPathCosts.GridSize));

				// ToDo
				/*
				if (mapPathCosts._terrainPathGrids.Count == 0)
				{
					continue;
				}

				int terrainPathGridSize = mapPathCosts._terrainPathGrids.First().Value.Length * sizeof(int);
				foreach (var pair in mapPathCosts._terrainPathGrids)
				{
					var label = DefDatabase<MovementDef>.AllDefsListForReading[pair.Key].LabelCap.ToString();
					var movementName = $"{label} terrain grid";
					report.Add(new MemoryUsageData(cacheName, mapName, movementName,
						MemoryUsageData.DictionaryPairSizeWithoutValue + terrainPathGridSize));
				}
				*/
			}

			return report;
		}
	}
}