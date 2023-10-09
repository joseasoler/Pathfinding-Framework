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
	public static class PathCosts
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
			foreach (var tag in terrainDef.tags)
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
		/// Set the PassableWithAnyMovement flag of any impassable TerrainDef that is passable with this movement.
		/// </summary>
		/// <param name="movementDef"></param>
		/// <returns></returns>
		public static short[] Update(MovementDef movementDef)
		{
			PathCost defaultCost = movementDef.defaultCost;
			short defaultCostAdd = movementDef.defaultCostAdd;
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			short[] result = new short[terrainDefs.Count];

			for (var terrainIndex = 0; terrainIndex < terrainDefs.Count; ++terrainIndex)
			{
				TerrainDef terrainDef = terrainDefs[terrainIndex];
				short maxTagCost = CalculateMaxTagCost(terrainDef, movementDef);
				int terrainPathCost = Math.Min(short.MaxValue, terrainDef.pathCost);
				short pathCost = CalculatePathCost(maxTagCost, terrainDef.passability, defaultCost, defaultCostAdd,
					(short) terrainPathCost);

				if (terrainDef.passability == Traversability.Impassable && pathCost < PathCost.Impassable.cost)
				{
					terrainDef.PassableWithAnyMovement() = true;
				}

				result[terrainIndex] = pathCost;
			}

			return result;
		}
	}
}