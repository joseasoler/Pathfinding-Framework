using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Cache.Local
{
	public class MapPathCosts : MapPathingData
	{
		/// <summary>
		/// Collection of multiple path costs related to each cell of the map.
		/// </summary>
		private readonly MapPathCost[] _mapGrid;

		/// <summary>
		/// Number of pawns with each movement. Used to determine which terrain path grids should be kept updated.
		/// </summary>
		private readonly short[] _pawnMovementCounts;

		/// <summary>
		/// Terrain path grids, indexed by MovementDef.index.
		/// </summary>
		private readonly Dictionary<int, short[]> _terrainPathGrids;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		public MapPathCosts(Map map) : base(map)
		{
			_mapGrid = new MapPathCost[GridSize];
			_pawnMovementCounts = new short[DefDatabase<MovementDef>.AllDefsListForReading.Count];
			_terrainPathGrids = new Dictionary<int, short[]>();
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
			_mapGrid[cellIndex].fire += (short) (spawned ? centerCellCost : -centerCellCost);

			var adjacentCells = GenAdj.AdjacentCells;
			for (int adjacentIndex = 0; adjacentIndex < adjacentCells.Length; ++adjacentIndex)
			{
				var adjacentCell = cell + adjacentCells[adjacentIndex];
				var adjacentCellIndex = ToIndex(adjacentCell);

				if (!InBounds(adjacentCellIndex))
				{
					continue;
				}

				const short adjacentCellCost = 150;
				_mapGrid[adjacentCellIndex].fire += (short) (spawned ? adjacentCellCost : -adjacentCellCost);
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

			var thingList = Map.thingGrid.ThingsListAtFast(cellIndex);
			for (int thingIndex = 0; thingIndex < thingList.Count; ++thingIndex)
			{
				var thing = thingList[thingIndex];

				if (thing.def.passability == Traversability.Impassable)
				{
					mapPathCostRef.things = (short) PathCostValues.Impassable;
					mapPathCostRef.nonIgnoreRepeaterThings = PathCost.Impassable.cost;
					break;
				}

				int currentThingCost = thing.def.pathCost;
				short narrowedThingPathCost =
					currentThingCost > PathCost.Impassable.cost ? PathCost.Impassable.cost : (short) currentThingCost;
				mapPathCostRef.things = Math.Max(mapPathCostRef.things, narrowedThingPathCost);

				if (!PathGrid.IsPathCostIgnoreRepeater(thing.def))
				{
					mapPathCostRef.nonIgnoreRepeaterThings =
						Math.Max(mapPathCostRef.nonIgnoreRepeaterThings, narrowedThingPathCost);
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
		/// <param name="movementDef">Movement to increase.</param>
		public void PawnAdded(MovementDef movementDef)
		{
			int movementIndex = movementDef.index;
			if (_pawnMovementCounts[movementIndex] == 0)
			{
				_terrainPathGrids.Add(movementIndex, new short[GridSize]);
				UpdateTerrainCostOfMovement(movementIndex);
			}

			++_pawnMovementCounts[movementIndex];
		}

		/// <summary>
		/// A new pawn has been removed from the map.
		/// </summary>
		/// <param name="movementDef">Movement to decrease.</param>
		public void PawnRemoved(MovementDef movementDef)
		{
			int movementIndex = movementDef.index;
			--_pawnMovementCounts[movementIndex];

			if (_pawnMovementCounts[movementIndex] == 0)
			{
				_terrainPathGrids.Remove(movementIndex);
			}
		}

		/// <summary>
		/// A new pawn has changed its movement type.
		/// </summary>
		/// <param name="previousMovementdef">Movement to decrease.</param>
		/// <param name="newMovementDef">Movement to increase.</param>
		public void PawnUpdated(MovementDef previousMovementdef, MovementDef newMovementDef)
		{
			PawnRemoved(previousMovementdef);
			PawnAdded(newMovementDef);
		}

		/// <summary>
		/// Update a specific position of every terrain path grid.
		/// </summary>
		/// <param name="cell">Cell being updated.</param>
		public void UpdateTerrainCost(IntVec3 cell)
		{
			var cellIndex = ToIndex(cell);
			var terrainIndex = Map.terrainGrid.TerrainAt(cellIndex).index;
			for (int movementIndex = 0; movementIndex < _pawnMovementCounts.Length; ++movementIndex)
			{
				if (_pawnMovementCounts[movementIndex] == 0)
				{
					continue;
				}

				// ToDo re-implement properly when pathing contexts are cached and contain terrain costs.
				// _terrainPathGrids[movementIndex][cellIndex] = MovementPathCostCache.Get(movementIndex, terrainIndex);
			}
		}

		private void UpdateTerrainCostOfMovement(int movementIndex)
		{
			var terrainPathGrid = _terrainPathGrids[movementIndex];
			for (int cellIndex = 0; cellIndex < GridSize; ++cellIndex)
			{
				var terrainIndex = Map.terrainGrid.TerrainAt(cellIndex).index;
				// ToDo re-implement properly when pathing contexts are cached and contain terrain costs.
				// terrainPathGrid[cellIndex] = MovementPathCostCache.Get(movementIndex, terrainIndex);
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
	}
}