using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Common implementation for classes which need to store some data for each cell of a map.
	/// </summary>
	public abstract class MapGrid
	{
		/// <summary>
		/// X size of the parent map. Stored to convert cells to indexes and other operations without the parent map.
		/// </summary>
		private int _mapSizeX;

		/// <summary>
		/// Total number of cells in the current map. Cached for performing InBounds checks without the parent map.
		/// </summary>
		public int GridSize;

		/// <summary>
		/// Reference to the current map. Intended for access to the thing grid and terrain cost grid.
		/// Avoid using expensive methods through it and prefer local utility methods instead.
		/// </summary>
		protected readonly Map Map;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		protected MapGrid(Map map)
		{
			GridSize = map.Size.x * map.Size.z;
			_mapSizeX = map.Size.x;
			Map = map;
		}

		/// <summary>
		/// In-class replacement for GenGrid.InBounds, using cell indexes.
		/// </summary>
		/// <param name="cellIndex">Cell index to be checked</param>
		/// <returns>True if the index refers to a cell inside of the map.</returns>
		protected bool InBounds(int cellIndex)
		{
			return cellIndex >= 0 && cellIndex < GridSize;
		}

		/// <summary>
		/// In-class replacement for Verse.CellIndices.CellToIndex.
		/// </summary>
		/// <param name="cell">Cell to convert.</param>
		/// <returns>Index of the cell in this map.</returns>
		protected int ToIndex(IntVec3 cell)
		{
			return cell.z * _mapSizeX + cell.x;
		}

		public string MapName()
		{
			return Map.GetUniqueLoadID();
		}
	}
}