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
		/// Collection of multiple path costs related to each cell of the map.
		/// </summary>
		private readonly MapPathCost[] _mapGrid;

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
		public static MapPathCostCache GetCache(int mapUniqueId)
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

			_mapGrid = new MapPathCost[_gridSize];
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

			const short centerCellCost = 1000;
			_mapGrid[cellIndex].fire += (short)(spawned ? centerCellCost : -centerCellCost);

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
				_mapGrid[adjacentCellIndex].fire += (short)(spawned ? adjacentCellCost : -adjacentCellCost);
			}
		}

		/// <summary>
		/// Update pathing costs related to things present in a cell.
		/// </summary>
		/// <param name="cell">Cell to be updated.</param>
		public void UpdateThings(IntVec3 cell)
		{
			var cellIndex = ToIndex(cell);

			ref var mapPathCostRef = ref _mapGrid[cellIndex];

			// Reset the current values.
			mapPathCostRef.things = 0;
			mapPathCostRef.nonIgnoreRepeaterThings = 0;
			mapPathCostRef.hasIgnoreRepeater = false;

			var thingList = _map.thingGrid.ThingsListAtFast(cellIndex);
			for (int thingIndex = 0; thingIndex < thingList.Count; ++thingIndex)
			{
				var thing = thingList[thingIndex];

				if (thing.def.passability == Traversability.Impassable)
				{
					mapPathCostRef.things = (int)PathCostValues.Impassable;
					mapPathCostRef.nonIgnoreRepeaterThings = (int)PathCostValues.Impassable;
					break;
				}

				var currentThingCost = (short)thing.def.pathCost;
				mapPathCostRef.things = Math.Max(mapPathCostRef.things, currentThingCost);

				if (!PathGrid.IsPathCostIgnoreRepeater(thing.def))
				{
					mapPathCostRef.nonIgnoreRepeaterThings = Math.Max(mapPathCostRef.nonIgnoreRepeaterThings, currentThingCost);
				}
				else
				{
					mapPathCostRef.hasIgnoreRepeater = true;
				}

				mapPathCostRef.hasFence = mapPathCostRef.hasFence || (thing.def.building != null && thing.def.building.isFence);
			}
		}

		/// <summary>
		/// Update the has door grid when a door spawns or de-spawns.
		/// </summary>
		/// <param name="cell">Position of the door</param>
		/// <param name="value">New value.</param>
		public void SetHasDoor(IntVec3 cell, bool value)
		{
			_mapGrid[ToIndex(cell)].hasDoor = value;
		}

		/// <summary>
		/// Get map path costs of a cell
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Path cost.</returns>
		public MapPathCost Get(int cellIndex)
		{
			return _mapGrid[cellIndex];
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

			var mapPathCostSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MapPathCost));

			foreach (var mapCache in GlobalMapCache)
			{
				var cache = mapCache.Value;
				var mapName = cache._map.GetUniqueLoadID();

				report.Add(new MemoryUsageData(cacheName, mapName, "Map path cost grid",
					mapPathCostSize * cache._mapGrid.Length));

				var terrainPathGridSize = cache._terrainPathGrids.First().Value.Length * sizeof(int);
				report.AddRange(cache._terrainPathGrids
					.Select(terrainPathGrid =>
						DefDatabase<MovementDef>.AllDefsListForReading[terrainPathGrid.Key].LabelCap.ToString()).Select(
						movementName => new MemoryUsageData(cacheName, mapName, $"{movementName} terrain grid",
							MemoryUsageData.DictionaryPairSizeWithoutValue + terrainPathGridSize)));
			}

			return report;
		}
	}
}