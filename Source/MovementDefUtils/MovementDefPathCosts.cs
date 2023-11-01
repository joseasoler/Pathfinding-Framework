using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Calculate path costs of terrains for a movement definition.
	/// </summary>
	public static class MovementDefPathCosts
	{
		/// <summary>
		/// Obtain the largest pathing cost of the tags of a specific terrain.
		/// </summary>
		/// <param name="terrainDef">Terrain being evaluated.</param>
		/// <param name="movementDef">Movement type being checked.</param>
		/// <returns>Default if no tag was found, the maximum pathing cost otherwise.</returns>
		private static short CalculateMaxTagCost(TerrainDef terrainDef, MovementDef movementDef)
		{
			var maxTagCost = PathCost.Default.cost;

			if (terrainDef.tags == null || movementDef.tagCosts == null)
			{
				return maxTagCost;
			}

			var pathCostData = movementDef.tagCosts.data;
			foreach (string tag in terrainDef.tags)
			{
				if (pathCostData.TryGetValue(tag, out var tagCost))
				{
					maxTagCost = Math.Max(maxTagCost, tagCost.cost);
				}
			}

			return maxTagCost;
		}

		/// <summary>
		/// Calculate the path cost to use for a specific terrain and movement type.
		/// </summary>
		/// <param name="maxTagCost">Maximum path cost of the tags of this terrain for this movement type.</param>
		/// <param name="passability">Vanilla traversability of this terrain.</param>
		/// <param name="defaultCost">Default cost defined in the movement type.</param>
		/// <param name="defaultCostAdd">Value to add to default cost.</param>
		/// <param name="terrainPathCost">Vanilla path cost.</param>
		/// <returns>Cost to use for pathing.</returns>
		private static short CalculatePathCost(short maxTagCost, Traversability passability, PathCost defaultCost,
			short defaultCostAdd, short terrainPathCost)
		{
			if (maxTagCost > PathCost.Default.cost)
			{
				return maxTagCost;
			}

			if (passability == Traversability.Impassable)
			{
				return PathCost.Impassable.cost;
			}

			short result = defaultCost != PathCost.Default
				? defaultCost.cost
				: Math.Min(terrainPathCost, PathCost.Impassable.cost);

			if (defaultCostAdd > 0)
			{
				result += defaultCostAdd;
			}

			return result;
		}

		/// <summary>
		/// Update the PathCosts array of a MovementDef.
		/// </summary>
		/// <param name="movementDef"></param>
		/// <returns>Array of path costs.</returns>
		private static void Update(MovementDef movementDef)
		{
			PathCost defaultCost = movementDef.defaultCost;
			short defaultCostAdd = movementDef.defaultCostAdd;
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			movementDef.PathCosts = new short[terrainDefs.Count];

			for (int terrainIndex = 0; terrainIndex < terrainDefs.Count; ++terrainIndex)
			{
				TerrainDef terrainDef = terrainDefs[terrainIndex];
				if (terrainDef.MovementIndex() != terrainIndex)
				{
					Report.Error(
						$"Index mismatch detected in {terrainDef}. Expected {terrainIndex}, got {terrainDef.MovementIndex()}.");
				}

				short maxTagCost = CalculateMaxTagCost(terrainDef, movementDef);
				int terrainPathCost = Math.Min(short.MaxValue, terrainDef.pathCost);
				short pathCost = CalculatePathCost(maxTagCost, terrainDef.passability, defaultCost, defaultCostAdd,
					(short)terrainPathCost);
				movementDef.PathCosts[terrainIndex] = pathCost;
			}
		}

		public static void Initialize()
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			for (int movementDefIndex = 0; movementDefIndex < movementDefs.Count; ++movementDefIndex)
			{
				Update(movementDefs[movementDefIndex]);
			}
		}
	}
}