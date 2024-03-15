using System;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapPathCosts;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.MovementContexts
{
	/// <summary>
	/// Pathing context information to use for specific pawns.
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

		/// <summary>
		/// Determines if the context should treat fences as impassable.
		/// </summary>
		public readonly bool ShouldAvoidFences;

		/// <summary>
		/// True if the pawn can ignore path costs related to fire. Only used if the option is enabled in mod settings.
		/// </summary>
		public readonly bool CanIgnoreFire;

		public MovementContext(MovementDef movementDef, Map map, bool shouldAvoidFences, bool ignoreFire) : base(map)
		{
			MovementDef = movementDef;
			PathGrid grid = new PathGrid(map, !shouldAvoidFences);
			PathingContext = new PathingContext(map, grid);
			ShouldAvoidFences = shouldAvoidFences;
			CanIgnoreFire = ignoreFire;
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
				cost = MovementDef.PathCosts[terrainDef.MovementIndex()];
				if (pathCosts.things > cost)
				{
					if (!MovementDef.ignoreThings || pathCosts.things >= PathCost.Unsafe.cost)
					{
						cost = pathCosts.things;
					}
				}

				if (!MovementDef.ignoreSnow && pathCosts.snow > cost)
				{
					cost = pathCosts.snow;
				}

				if (!Settings.Values.IgnoreFire || !CanIgnoreFire)
				{
					cost += pathCosts.fire;
				}
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

			int cost = MovementDef.PathCosts[terrainDef.MovementIndex()];
			cost = VanillaFurnitureExpandedSecurityCompat.MoveIntoCellTerrainCost(cost, terrainDef, prevCell, Map);
			MapPathCost prevMapPathCost = Map.MapPathCostGrid().Get(ToIndex(prevCell));
			short thingsCost = prevMapPathCost.hasIgnoreRepeater
				? nextMapPathCost.nonIgnoreRepeaterThings
				: nextMapPathCost.things;
			if (thingsCost > cost)
			{
				if (!MovementDef.ignoreThings || thingsCost >= PathCost.Unsafe.cost)
				{
					cost = thingsCost;
				}
			}

			if (!MovementDef.ignoreSnow && nextMapPathCost.snow > cost)
			{
				cost = nextMapPathCost.snow;
			}

			if (prevMapPathCost.hasDoor && nextMapPathCost.hasDoor)
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
			return terrainDef != null && MovementDef.CanEnterTerrain(terrainDef);
		}

		/// <summary>
		/// True if a pawn with this movement can stand in a specific cell.
		/// See GenGrid.Standable.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>Standability of the cell.</returns>
		public bool CanStandAt(IntVec3 cell)
		{
			return LocationFinding.CanStandAt(MovementDef, Map, cell);
		}

		/// <summary>
		/// Checks if pawns using this movement should avoid wandering at a cell.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the path cost is equal or greater than in vanilla and the terrain has avoidWander.</returns>
		public bool AvoidWanderAt(IntVec3 cell)
		{
			if (MovementDef.ignoreAvoidWander)
			{
				return false;
			}

			TerrainDef terrainDef = TerrainAt(cell);
			return terrainDef == null || terrainDef.avoidWander;
		}
	}
}