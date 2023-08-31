using Verse;

namespace PathfindingFramework.Cache.Local
{
	public class MapCache
	{
		/// <summary>
		/// X size of the parent map. Stored to convert cells to indexes and other operations without the parent map.
		/// </summary>
		private int _mapSizeX;

		/// <summary>
		/// Total number of cells in the current map. Cached for performing InBounds checks without the parent map.
		/// </summary>
		protected int GridSize;

		/// <summary>
		/// Reference to the current map. Intended for access to the thing grid and terrain cost grid.
		/// Avoid using expensive methods through it and prefer local utility methods instead.
		/// </summary>
		protected readonly Map Map;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		protected MapCache(Map map)
		{
			GridSize = map.Size.x * map.Size.z;
			_mapSizeX = map.Size.x;
			Map = map;
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
	}
}