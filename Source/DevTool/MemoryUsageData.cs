namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Contains estimations of memory usage. Used to get a general idea of memory usage and detect regressions
	/// and bugs. If you want to properly and exactly profile memory usage, use a better tool.
	/// </summary>
	public struct MemoryUsageData
	{
		/// <summary>
		/// Name of the cache.
		/// </summary>
		public readonly string Cache;

		/// <summary>
		/// Map of the cache, or global.
		/// </summary>
		public readonly string Map;

		/// <summary>
		/// One of the grids of the cache.
		/// </summary>
		public readonly string Grid;

		/// <summary>
		/// Approximation of the number of bytes in this data structure.
		/// </summary>
		public readonly int Bytes;

		public const string Global = "Global";

		/// <summary>
		/// Dictionaries are estimated as taking 8 bytes to point to the key, another 8 for the key and another 8 to point to the value.
		/// sizeof(long) is used as a replacement of sizeof(int*).
		/// </summary>
		public const int DictionaryPairSizeWithoutValue = 3 * sizeof(long);

		public MemoryUsageData(string cache, string map, string grid, int bytes)
		{
			Cache = cache;
			Map = map;
			Grid = grid;
			Bytes = bytes;
		}
	}
}