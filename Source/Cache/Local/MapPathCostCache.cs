using System;
using System.Collections.Generic;
using PathfindingFramework.Cache.Global;
using PathfindingFramework.Parse;
using Verse;
using Verse.AI;

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
		/// Reference to the current map. Avoid using expensive methods through it and prefer local utility methods instead.
		/// </summary>
		private readonly Map _map;

		/// <summary>
		/// Keeps track of pathfinding costs associated with fire presence.
		/// In vanilla, Verse.AI.PathGrid.CalculatedCostAt calculates this cost when perceivedStatic is set to true.
		/// In the Pathfinding Framework, a precalculated copy of this information is used to calculate all path grids.
		/// PF ignores costs from fires attached to pawns to improve performance.
		/// </summary>
		private readonly int[] _fireGrid;

		/// <summary>
		/// Path cost of things contained in the cell.
		/// Follows the same logic as Verse.AI.PathGrid.CalculatedCostAt for generating the pathCost in the first
		/// ThingsListAt loop.
		/// </summary>
		private readonly int[] _thingGrid;

		/// <summary>
		/// Path cost of things with IsPathCostIgnoreRepeater returning false in the cell.
		/// Follows the same logic as Verse.AI.PathGrid.CalculatedCostAt for generating the pathCost in the first
		/// ThingsListAt loop.
		/// </summary>
		private readonly int[] _nonIgnoreRepeaterThingGrid;

		/// <summary>
		/// True if the cell contains one or more things with IsPathCostIgnoreRepeater returning true.
		/// </summary>
		private readonly bool[] _hasIgnoreRepeaterGrid;

		/// <summary>
		/// True for cells containing a door.
		/// Used to replicate Verse.AI.PathGrid.CalculatedCostAt logic to increase path costs if both the previous and
		/// current tiles have doors.
		/// </summary>
		private readonly bool[] _hasDoorGrid;

		/// <summary>
		/// Number of pawns with each movement. Used to determine which terrain grids should be kept updated.
		/// </summary>
		private readonly int[] _pawnMovementCounts;

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
			_gridSize = map.Size.x * map.Size.z;
			_mapSizeX = map.Size.x;
			_map = map;

			_fireGrid = new int[_gridSize];
			_thingGrid = new int[_gridSize];
			_nonIgnoreRepeaterThingGrid = new int[_gridSize];
			_hasIgnoreRepeaterGrid = new bool[_gridSize];
			_hasDoorGrid = new bool[_gridSize];
			_pawnMovementCounts = new int[MovementPathCostCache.MovementCount()];
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
		/// <param name="spawned">True for spawning fires, false for de-spawning fires.</param>
		public void UpdateFire(IntVec3 cell, bool spawned)
		{
			var cellIndex = ToIndex(cell);
			if (!InBounds(cellIndex))
			{
				return;
			}

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

		/// <summary>
		/// Update pathing costs related to things present in a cell.
		/// </summary>
		/// <param name="cell">Cell to be updated.</param>
		public void UpdateThings(IntVec3 cell)
		{
			var cellIndex = ToIndex(cell);
			// Get references to the relevant values of this cell.
			ref int thingCostRef = ref _thingGrid[cellIndex];
			ref int nonIgnoreRepeaterThingCostRef = ref _nonIgnoreRepeaterThingGrid[cellIndex];
			ref bool hasIgnoreRepeaterRef = ref _hasIgnoreRepeaterGrid[cellIndex];

			// Reset the current values.
			thingCostRef = 0;
			nonIgnoreRepeaterThingCostRef = 0;
			hasIgnoreRepeaterRef = false;

			var thingList = _map.thingGrid.ThingsListAtFast(cellIndex);
			for (int thingIndex = 0; thingIndex < thingList.Count; ++thingIndex)
			{
				var thing = thingList[thingIndex];

				if (thing.def.passability == Traversability.Impassable)
				{
					thingCostRef = (int)PathCostValues.Impassable;
					nonIgnoreRepeaterThingCostRef = (int)PathCostValues.Impassable;
					break;
				}

				var currentThingCost = thing.def.pathCost;
				thingCostRef = Math.Max(thingCostRef, currentThingCost);

				if (!PathGrid.IsPathCostIgnoreRepeater(thing.def))
				{
					nonIgnoreRepeaterThingCostRef = Math.Max(nonIgnoreRepeaterThingCostRef, currentThingCost);
				}
				else
				{
					hasIgnoreRepeaterRef = true;
				}
			}
		}

		/// <summary>
		/// Path cost of things in the cell.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Path cost.</returns>
		public int ThingsCost(int cellIndex)
		{
			return _thingGrid[cellIndex];
		}

		/// <summary>
		/// Path cost of things with IsPathCostIgnoreRepeater returning false in the cell.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Path cost.</returns>
		public int NonIgnoreRepeaterThingsCost(int cellIndex)
		{
			return _nonIgnoreRepeaterThingGrid[cellIndex];
		}

		/// <summary>
		/// True if the cell contains one or more things with IsPathCostIgnoreRepeater returning true.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Boolean flag.</returns>
		public bool HasIgnoreRepeater(int cellIndex)
		{
			return _hasIgnoreRepeaterGrid[cellIndex];
		}

		/// <summary>
		/// Update the has door grid when a door spawns or de-spawns.
		/// </summary>
		/// <param name="cell">Position of the door</param>
		/// <param name="value">New value.</param>
		public void SetHasDoor(IntVec3 cell, bool value)
		{
			_hasDoorGrid[ToIndex(cell)] = value;
		}

		/// <summary>
		/// True if the cell contains a door.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Boolean flag.</returns>
		public bool HasDoor(int cellIndex)
		{
			return _hasDoorGrid[cellIndex];
		}

		/// <summary>
		/// A new pawn has spawned in this map.
		/// </summary>
		/// <param name="movementIndex">Movement index to increase.</param>
		public void PawnSpawned(int movementIndex)
		{
			++_pawnMovementCounts[movementIndex];
		}

		/// <summary>
		/// A new pawn has changed its movement type.
		/// </summary>
		/// <param name="previousMovementIndex">Movement index to decrease.</param>
		/// <param name="newMovementIndex">Movement index to increase.</param>
		public void PawnMovementChanged(int previousMovementIndex, int newMovementIndex)
		{
			--_pawnMovementCounts[previousMovementIndex];
			++_pawnMovementCounts[newMovementIndex];
		}

		/// <summary>
		/// A new pawn has been removed from the map.
		/// </summary>
		/// <param name="movementIndex">Movement index to decrease.</param>
		public void PawnDespawned(int movementIndex)
		{
			--_pawnMovementCounts[movementIndex];
		}
	}
}