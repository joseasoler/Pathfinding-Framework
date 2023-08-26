using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.Cache.Local
{
	/// <summary>
	/// Stores and keeps updated different pathfinding grids for a specific map.
	/// The life cycle of this class is managed by being a MapComponent, but for performance reasons access requests
	/// should go through the global map cache.
	/// </summary>
	public class MapPathCostCache : MapComponent
	{
		/// <summary>
		/// Associates a map path cost cache to each map.uniqueID value.
		/// </summary>
		private static readonly Dictionary<int, MapPathCostCache> GlobalMapCache = new Dictionary<int, MapPathCostCache>();

		/// <summary>
		/// Unique map ID of the parent map. Used to keep the global map cache updated.
		/// </summary>
		private readonly int _mapUniqueId;

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
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		public MapPathCostCache(Map map) : base(map)
		{
			_mapUniqueId = map.uniqueID;
		}

		/// <summary>
		/// Start accepting pathfinding update calls after the map is fully initialized.
		/// </summary>
		public override void FinalizeInit()
		{
			GlobalMapCache.Add(_mapUniqueId, this);
			// ToDo RecalculateAllPerceivedPathCosts
		}

		/// <summary>
		/// Clean up the global map cache and stop accepting pathfinding update calls.
		/// </summary>
		public override void MapRemoved()
		{
			GlobalMapCache.Remove(_mapUniqueId);
		}
	}
}