﻿using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Reference to the current map. Intended for access to the thing grid and terrain cost grid.
		/// Avoid using expensive methods through it and prefer local utility methods instead.
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
		/// Keeps track of fence presence.
		/// In vanilla, Verse.AI.PathGrid.CalculatedCostAt considers fences as impassable in the fenceBlocked pathing.
		/// </summary>
		private readonly bool[] _hasFenceGrid;

		/// <summary>
		/// Number of pawns with each movement. Used to determine which terrain path grids should be kept updated.
		/// </summary>
		private readonly int[] _pawnMovementCounts;

		/// <summary>
		/// Terrain path grids, indexed by MovementDef.index.
		/// </summary>
		private readonly Dictionary<int, int[]> _terrainPathGrids;

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
			_hasFenceGrid = new bool[_gridSize];
			_pawnMovementCounts = new int[MovementPathCostCache.MovementCount()];
			_terrainPathGrids = new Dictionary<int, int[]>();
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

			const int centerCellCost = 1000;
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
			ref bool hasFenceRef = ref _hasFenceGrid[cellIndex];

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

				hasFenceRef = hasFenceRef || (thing.def.building != null && thing.def.building.isFence);
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
		/// <param name="cellIndex">Cell to check.</param>
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
		/// <param name="cellIndex">Cell to check.</param>
		/// <returns>Boolean flag.</returns>
		public bool HasDoor(int cellIndex)
		{
			return _hasDoorGrid[cellIndex];
		}

		/// <summary>
		/// True if the cell contains a fence.
		/// </summary>
		/// <param name="cellIndex">Cell to check.</param>
		/// <returns>Boolean flag.</returns>
		public bool HasFence(int cellIndex)
		{
			return _hasFenceGrid[cellIndex];
		}

		/// <summary>
		/// A new pawn has spawned in this map.
		/// </summary>
		/// <param name="movementIndex">Movement index to increase.</param>
		public void PawnAdded(int movementIndex)
		{
			if (_pawnMovementCounts[movementIndex] == 0)
			{
				_terrainPathGrids.Add(movementIndex, new int[_gridSize]);
				UpdateTerrainCostOfMovement(movementIndex);
			}

			++_pawnMovementCounts[movementIndex];
		}

		/// <summary>
		/// A new pawn has been removed from the map.
		/// </summary>
		/// <param name="movementIndex">Movement index to decrease.</param>
		public void PawnRemoved(int movementIndex)
		{
			--_pawnMovementCounts[movementIndex];

			if (_pawnMovementCounts[movementIndex] == 0)
			{
				_terrainPathGrids.Remove(movementIndex);
			}
		}

		/// <summary>
		/// A new pawn has changed its movement type.
		/// </summary>
		/// <param name="previousMovementIndex">Movement index to decrease.</param>
		/// <param name="newMovementIndex">Movement index to increase.</param>
		public void PawnUpdated(int previousMovementIndex, int newMovementIndex)
		{
			PawnRemoved(previousMovementIndex);
			PawnAdded(newMovementIndex);
		}

		/// <summary>
		/// Update a specific position of every terrain path grid.
		/// </summary>
		/// <param name="cell">Cell being updated.</param>
		public void UpdateTerrainCost(IntVec3 cell)
		{
			var cellIndex = ToIndex(cell);
			var terrainIndex = _map.terrainGrid.TerrainAt(cellIndex).index;
			for (int movementIndex = 0; movementIndex < _pawnMovementCounts.Length; ++movementIndex)
			{
				if (_pawnMovementCounts[movementIndex] == 0)
				{
					continue;
				}

				_terrainPathGrids[movementIndex][cellIndex] = MovementPathCostCache.Get(movementIndex, terrainIndex);
			}
		}

		private void UpdateTerrainCostOfMovement(int movementIndex)
		{
			var terrainPathGrid = _terrainPathGrids[movementIndex];
			for (int cellIndex = 0; cellIndex < _gridSize; ++cellIndex)
			{
				var terrainIndex = _map.terrainGrid.TerrainAt(cellIndex).index;
				terrainPathGrid[cellIndex] = MovementPathCostCache.Get(movementIndex, terrainIndex);
			}
		}

		/// <summary>
		/// Update all terrain grids.
		/// </summary>
		public void UpdateAllTerrainCosts()
		{
			for (int movementIndex = 0; movementIndex < _pawnMovementCounts.Length; ++movementIndex)
			{
				if (_pawnMovementCounts[movementIndex] == 0)
				{
					continue;
				}

				UpdateTerrainCostOfMovement(movementIndex);
			}
		}

		/// <summary>
		/// This function is intended for DevTool features only. Pawns should never need to check if their movement type
		/// is cached, it should always be there.
		/// </summary>
		/// <param name="movementIndex">Movement type to check.</param>
		/// <returns>True if the movement type is cached.</returns>
		public bool HasMovementType(int movementIndex)
		{
			return _pawnMovementCounts[movementIndex] != 0;
		}

		/// <summary>
		/// Pathfinding cost of terrain at a specific cell index for a certain movement type.
		/// </summary>
		/// <param name="movementType">Current movement type.</param>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Terrain path cost.</returns>
		public int TerrainCost(int movementType, int cellIndex)
		{
			return _terrainPathGrids[movementType][cellIndex];
		}

		public static List<MemoryUsageData> MemoryReport()
		{
			var cacheName = nameof(MapPathCostCache);
			var report = new List<MemoryUsageData>();

			foreach (var mapCache in GlobalMapCache)
			{
				var cache = mapCache.Value;
				var mapName = cache._map.GetUniqueLoadID();

				report.Add(new MemoryUsageData(cacheName, mapName, "Fire grid",
					sizeof(int) * cache._fireGrid.Length));
				report.Add(new MemoryUsageData(cacheName, mapName, "Things grid",
					sizeof(int) * cache._thingGrid.Length));
				report.Add(new MemoryUsageData(cacheName, mapName, "Non repeater things grid",
					sizeof(int) * cache._nonIgnoreRepeaterThingGrid.Length));
				report.Add(new MemoryUsageData(cacheName, mapName, "Has ignore repeater grid",
					sizeof(byte) * cache._hasIgnoreRepeaterGrid.Length));
				report.Add(new MemoryUsageData(cacheName, mapName, "Has door grid",
					sizeof(byte) * cache._hasDoorGrid.Length));
				report.Add(new MemoryUsageData(cacheName, mapName, "Has fence grid",
					sizeof(byte) * cache._hasFenceGrid.Length));

				var terrainPathGridSize = cache._terrainPathGrids.First().Value.Length * sizeof(int);

				foreach (var terrainPathGrid in cache._terrainPathGrids)
				{
					var movementName = DefDatabase<MovementDef>.AllDefsListForReading[terrainPathGrid.Key].LabelCap.ToString();
					report.Add(new MemoryUsageData(cacheName, mapName, $"{movementName} terrain grid",
						MemoryUsageData.DictionaryPairSizeWithoutValue + terrainPathGridSize));
				}
			}

			return report;
		}
	}
}