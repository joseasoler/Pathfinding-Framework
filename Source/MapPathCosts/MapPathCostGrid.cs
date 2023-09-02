using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using Verse;
using Verse.AI;

namespace PathfindingFramework.MapPathCosts
{
	public class MapPathCostGrid : MapGrid
	{
		/// <summary>
		/// Collection of multiple path costs related to each cell of the map.
		/// </summary>
		private readonly MapPathCosts.MapPathCost[] _mapGrid;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		public MapPathCostGrid(Map map) : base(map)
		{
			_mapGrid = new MapPathCosts.MapPathCost[GridSize];
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

		public void UpdateSnow(IntVec3 cell, int cost)
		{
			_mapGrid[ToIndex(cell)].snow = (sbyte) cost;
		}

		public void UpdateAllSnow()
		{
			for (int cellIndex = 0; cellIndex < GridSize; ++cellIndex)
			{
				float depth = Map.snowGrid.depthGrid[cellIndex];
				SnowCategory newCategory = SnowUtility.GetSnowCategory(depth);
				_mapGrid[cellIndex].snow = (sbyte) SnowUtility.MovementTicksAddOn(newCategory);
			}
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
	}
}