using System;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.MapPathCosts
{
	/// <summary>
	/// Keeps different map path costs up to date. These costs are shared between movement contexts of the same map.
	/// </summary>
	public class MapPathCostGrid : MapGrid
	{
		/// <summary>
		/// Collection of multiple path costs related to each cell of the map.
		/// </summary>
		private readonly MapPathCost[] _mapGrid;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		/// <param name="map">Parent map of this cache.</param>
		public MapPathCostGrid(Map map) : base(map)
		{
			_mapGrid = new MapPathCost[GridSize];
		}

		/// <summary>
		/// Update fire path costs after a fire instance is created or removed.
		/// </summary>
		/// <param name="cell">Position of the fire.</param>
		/// <param name="isSpawning">True for spawning fires, false for de-spawning fires.</param>
		public void UpdateFire(IntVec3 cell, bool isSpawning)
		{
			var cellIndex = ToIndex(cell);
			if (!InBounds(cellIndex))
			{
				return;
			}

			const short centerCellCost = 1000;
			_mapGrid[cellIndex].fire += (short) (isSpawning ? centerCellCost : -centerCellCost);
			Map.MovementContextData().UpdateCell(cellIndex);

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
				_mapGrid[adjacentCellIndex].fire += (short) (isSpawning ? adjacentCellCost : -adjacentCellCost);
				Map.MovementContextData().UpdateCell(adjacentIndex);
			}
		}

		/// <summary>
		/// Update path costs related to things present in a cell.
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
			mapPathCostRef.hasFence = false;

			var thingList = Map.thingGrid.ThingsListAtFast(cellIndex);
			for (int thingIndex = 0; thingIndex < thingList.Count; ++thingIndex)
			{
				var thing = thingList[thingIndex];

				if (thing.def.passability == Traversability.Impassable)
				{
					mapPathCostRef.things = PathCost.Impassable.cost;
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

			Map.MovementContextData().UpdateCell(cellIndex);
		}

		/// <summary>
		/// Update the has door grid when a door spawns or de-spawns.
		/// </summary>
		/// <param name="cell">Position of the door</param>
		/// <param name="isSpawning">True if the door has appeared. False if it has been removed.</param>
		public void SetHasDoor(IntVec3 cell, bool isSpawning)
		{
			int cellIndex = ToIndex(cell);
			_mapGrid[cellIndex].hasDoor = isSpawning;
			Map.MovementContextData().UpdateCell(cellIndex);
		}

		/// <summary>
		/// Updates path costs caused by snow.
		/// </summary>
		/// <param name="cell">Cell being updated.</param>
		/// <param name="cost">New path cost of snow.</param>
		public void UpdateSnow(IntVec3 cell, int cost)
		{
			int cellIndex = ToIndex(cell);
			_mapGrid[cellIndex].snow = (sbyte) cost;
			Map.MovementContextData().UpdateCell(cellIndex);
		}

		/// <summary>
		/// Update path costs related to snow in every cell of the map.
		/// </summary>
		public void UpdateSnowAllCells()
		{
			for (int cellIndex = 0; cellIndex < GridSize; ++cellIndex)
			{
				float depth = Map.snowGrid.depthGrid[cellIndex];
				SnowCategory newCategory = SnowUtility.GetSnowCategory(depth);
				_mapGrid[cellIndex].snow = (sbyte) SnowUtility.MovementTicksAddOn(newCategory);
				// This function does not need to update the MovementContextData. This is done separately by the caller.
			}
		}

		/// <summary>
		/// Get the map path costs of a cell.
		/// </summary>
		/// <param name="cellIndex">Index to check.</param>
		/// <returns>Path cost.</returns>
		public MapPathCost Get(int cellIndex)
		{
			return _mapGrid[cellIndex];
		}
	}
}