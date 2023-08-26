using System.Collections.Generic;
using PathfindingFramework.Parse;
using Verse;

namespace PathfindingFramework.Cache.Local
{
	/// <summary>
	/// Stores and keeps updated different pathfinding grids for a specific map.
	/// </summary>
	public class MapPathCostCache
	{
		/// <summary>
		/// Associates a map path cost cache to each map.uniqueID value.
		/// </summary>
		private static readonly Dictionary<int, MapPathCostCache> GlobalMapCache = new Dictionary<int, MapPathCostCache>();

		/// <summary>
		/// X size of the parent map. Stored to convert cells to indexes and other operations without the parent map.
		/// </summary>
		private int _mapSizeX;

		/// <summary>
		/// Total number of positions in the current map. Cached for performing InBounds checks without the parent map.
		/// </summary>
		private int _gridSize;

		/// <summary>
		/// Keeps track of pathfinding costs associated with fire presence.
		/// In vanilla, Verse.AI.PathGrid.CalculatedCostAt calculates this cost when perceivedStatic is set to true.
		/// In the Pathfinding Framework, a precalculated copy of this information is used to calculate all path grids.
		/// PF ignores costs from fires attached to pawns to improve performance.
		/// </summary>
		private readonly int[] _fireGrid;

		/// <summary>
		/// Obtain the map path cost cache of a specific map.
		/// </summary>
		/// <param name="mapUniqueId">Unique ID of the map being requested.</param>
		/// <returns>Map path cost cache, or null if the map is being initialized or destroyed.</returns>
		public static MapPathCostCache Get(int mapUniqueId)
		{
			return GlobalMapCache.TryGetValue(mapUniqueId, out var result) ? result : null;
		}

		/// <summary>
		/// Clear all map path cost caches.
		/// </summary>
		public static void Clear()
		{
			GlobalMapCache.Clear();
		}

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		public MapPathCostCache(Map map)
		{
			_gridSize = map.cellIndices.NumGridCells;
			_mapSizeX = map.Size.x;
			_fireGrid = new int[_gridSize];

			// ToDo initialize grids which do not have a default cost of zero.
		}

		/// <summary>
		/// Start accepting pathfinding update calls after the map is fully initialized.
		/// </summary>
		public static void Add(Map map)
		{
			GlobalMapCache.Add(map.uniqueID, new MapPathCostCache(map));
		}

		/// <summary>
		/// Clean up the global map cache and stop accepting pathfinding update calls.
		/// </summary>
		public static void Remove(Map map)
		{
			GlobalMapCache.Remove(map.uniqueID);
		}

		/// <summary>
		/// In-class replacement for Verse.CellIndices.CellToIndex.
		/// </summary>
		/// <param name="cell">Cell to convert.</param>
		/// <returns>Index of the cell in this map.</returns>
		private int ToIndex(IntVec3 cell)
		{
			return cell.z * _mapSizeX + cell.x;
		}

		/// <summary>
		/// In-class replacement for GenGrid.InBounds, using cell indexes.
		/// </summary>
		/// <param name="cellIndex">Cell index to be checked</param>
		/// <returns>True if the index refers to a cell inside of the map.</returns>
		private bool InBounds(int cellIndex)
		{
			return cellIndex >= 0 && cellIndex < _gridSize;
		}

		/// <summary>
		/// Update the fire grid after a fire instance is created or removed.
		/// </summary>
		/// <param name="cell">Position of the fire.</param>
		/// <param name="spawned">True for spawning fires, false for despawning fires.</param>
		public void UpdateFire(IntVec3 cell, bool spawned)
		{
			var cellIndex = ToIndex(cell);
			if (!InBounds(cellIndex))
			{
				return;
			}

			// See Verse.AI.PathGrid.CalculatedCostAt for the original implementation.
			const int centerCellCost = (int)PathCostValues.Impassable;
			_fireGrid[cellIndex] += spawned ? centerCellCost : -centerCellCost;

			var adjacentCells = GenAdj.AdjacentCells;
			for (int adjacentIndex = 0; adjacentIndex < adjacentCells.Length; ++adjacentIndex)
			{
				var adjacentCell = cell + adjacentCells[adjacentIndex];
				var adjacentCellIndex = ToIndex(adjacentCell);

				if (!InBounds(adjacentCellIndex))
				{
					continue;
				}

				const int adjacentCellCost = 150;
				_fireGrid[adjacentCellIndex] += spawned ? adjacentCellCost : -adjacentCellCost;
			}
		}

		/// <summary>
		/// Pathfinding cost of fire at a specific cell index.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Fire path cost.</returns>
		public int FireCost(int cellIndex)
		{
			return _fireGrid[cellIndex];
		}
	}
}