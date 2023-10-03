using System;
using System.Collections.Generic;
using PathfindingFramework.MapPathCosts;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.MovementContexts
{
	/// <summary>
	/// Context information to use for pawn pathfinding.
	/// </summary>
	public class MovementContext : MapGrid
	{
		/// <summary>
		/// Movement definition of this context.
		/// </summary>
		public readonly MovementDef MovementDef;

		/// <summary>
		/// Stores a pathing context instance ready to be used by the vanilla pathfinding code.
		/// Like in vanilla, the path grid in this context stores perceived pathfinding costs.
		/// </summary>
		public readonly PathingContext PathingContext;

		public readonly bool ShouldAvoidFences;

		public MovementContext(MovementDef movementDef, Map map, bool shouldAvoidFences) : base(map)
		{
			MovementDef = movementDef;
			PathGrid grid = new PathGrid(map, !shouldAvoidFences);
			PathingContext = new PathingContext(map, grid);
			ShouldAvoidFences = shouldAvoidFences;
		}

		/// <summary>
		/// Equivalent to calling PathGrid.CalculatedCostAt with perceivedStatic set to true and no prevCell.
		/// </summary>
		/// <param name="cellIndex">Index of the cell being calculated</param>
		/// <param name="pathCosts">Cached path costs for this cell.</param>
		public void UpdateCell(int cellIndex, MapPathCost pathCosts)
		{
			TerrainDef terrainDef = Map.terrainGrid.TerrainAt(cellIndex);

			int cost = PathCost.Impassable.cost;
			if (terrainDef != null && (!ShouldAvoidFences || !pathCosts.hasFence))
			{
				cost = MovementDef.PathCosts[terrainDef.index];
				if (pathCosts.things > cost)
				{
					if (!MovementDef.ignoreThings || pathCosts.things >= PathCost.Avoid.cost)
					{
						cost = pathCosts.things;
					}
				}

				if (!MovementDef.ignoreSnow && pathCosts.snow > cost)
				{
					cost = pathCosts.snow;
				}

				cost += pathCosts.fire;
			}

			cost = Math.Min(cost, PathCost.Impassable.cost);
			PathingContext.pathGrid.pathGrid[cellIndex] = cost;
		}

		/// <summary>
		/// Equivalent to calling PathGrid.CalculatedCostAt with perceivedStatic set to false and a prevCell.
		/// </summary>
		/// <param name="prevCell">Previous cell of the movement.</param>
		/// <param name="nextCell">Next cell of the movement.</param>
		/// <returns></returns>
		public int MoveIntoCellCost(IntVec3 prevCell, IntVec3 nextCell)
		{
			int nextCellIndex = ToIndex(nextCell);
			MapPathCost nextMapPathCost = Map.MapPathCostGrid().Get(nextCellIndex);
			TerrainDef terrainDef = Map.terrainGrid.TerrainAt(nextCellIndex);
			if (terrainDef == null || (ShouldAvoidFences && nextMapPathCost.hasFence))
			{
				return PathCost.Impassable.cost;
			}

			int cost = MovementDef.PathCosts[terrainDef.index];
			MapPathCost prevMapPathCost = Map.MapPathCostGrid().Get(ToIndex(prevCell));
			short thingsCost = prevMapPathCost.hasIgnoreRepeater
				? nextMapPathCost.nonIgnoreRepeaterThings
				: nextMapPathCost.things;
			if (thingsCost > cost)
			{
				if (!MovementDef.ignoreThings || thingsCost >= PathCost.Avoid.cost)
				{
					cost = thingsCost;
				}
			}

			if (!MovementDef.ignoreSnow && nextMapPathCost.snow > cost)
			{
				cost = nextMapPathCost.snow;
			}

			if (prevMapPathCost.hasDoor)
			{
				cost += 45;
			}

			cost = Math.Min(cost, PathCost.Impassable.cost);
			return cost;
		}

		private TerrainDef TerrainAt(IntVec3 cell)
		{
			int cellIndex = ToIndex(cell);
			return (cellIndex < 0 || cellIndex >= GridSize) ? null : Map.terrainGrid.TerrainAt(cellIndex);
		}

		/// <summary>
		/// Checks if a cell can be entered by pawns with this movement context.
		/// Only the terrain is considered for this check, as pawns must be able to reach impassable tiles with a wall.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the cell is passable.</returns>
		public bool CanEnterTerrain(IntVec3 cell)
		{
			TerrainDef terrainDef = TerrainAt(cell);

			if (terrainDef == null)
			{
				return false;
			}

			int cost = MovementDef.PathCosts[terrainDef.index];
			return cost < PathCost.Avoid.cost;
		}

		/// <summary>
		/// True if a pawn with this movement can stand in a specific cell.
		/// See GenGrid.Standable.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>Standability of the cell.</returns>
		public bool CanStandAt(IntVec3 cell)
		{
			if (!CanEnterTerrain(cell))
			{
				return false;
			}

			List<Thing> thingList = Map.thingGrid.ThingsListAt(cell);
			for (int index = 0; index < thingList.Count; ++index)
			{
				if (thingList[index].def.passability != Traversability.Standable)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks if pawns using this movement should avoid wandering at a cell.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the path cost is equal or greater than in vanilla and the terrain has avoidWander.</returns>
		public bool AvoidWanderAt(IntVec3 cell)
		{
			TerrainDef terrainDef = TerrainAt(cell);

			if (terrainDef == null)
			{
				return true;
			}

			int cost = MovementDef.PathCosts[terrainDef.index];
			return cost >= terrainDef.pathCost && terrainDef.avoidWander;
		}
	}
}