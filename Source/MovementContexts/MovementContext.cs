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
		public MovementDef MovementDef;

		/// <summary>
		/// Stores a pathing context instance ready to be used by the vanilla pathfinding code.
		/// Like in vanilla, the path grid in this context stores perceived pathfinding costs.
		/// </summary>
		public PathingContext PathingContext;

		public bool ShouldAvoidFences;

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
				if (cost < PathCost.Impassable.cost)
				{
					if (pathCosts.things > cost)
					{
						cost = pathCosts.things;
					}

					if (pathCosts.snow > cost)
					{
						cost = pathCosts.snow;
					}

					cost += pathCosts.fire;
				}
			}

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
			if (cost < PathCost.Impassable.cost)
			{
				short thingsCost = prevMapPathCost.hasIgnoreRepeater
					? nextMapPathCost.nonIgnoreRepeaterThings
					: nextMapPathCost.things;
				if (thingsCost > cost)
				{
					cost = thingsCost;
				}

				if (nextMapPathCost.snow > cost)
				{
					cost = nextMapPathCost.snow;
				}

				if (prevMapPathCost.hasDoor)
				{
					cost += 45;
				}
			}

			return cost;
		}

		/// <summary>
		/// Checks if a cell can be entered by pawns with this movement context.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the cell is passable.</returns>
		public bool CanEnter(IntVec3 cell)
		{
			return PathingContext.pathGrid.pathGrid[ToIndex(cell)] <= PathCost.Impassable.cost;
		}
	}
}