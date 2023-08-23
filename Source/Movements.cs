using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Parses movement types from XML and stores them in memory, allowing faster access and usage.
	/// This class relies on Defs having an index field that starts at zero. 
	/// </summary>
	public class Movements
	{
		/// <summary>
		/// Stores a list of terrain path costs for each movement type.
		/// </summary>
		private static int[] _costs;

		/// <summary>
		/// Number of TerrainDefs loaded in the game.
		/// </summary>
		private static int _terrainCount;


		/// <summary>
		/// Obtain the largest pathing cost of the tags of a specific terrain.  
		/// </summary>
		/// <param name="terrain">Terrain being evaluated</param>
		/// <param name="pathCostData">Pathing cost of each tag for a certain movement type.</param>
		/// <returns>Default if no tag was found, the maximum pathing cost otherwise.</returns>
		private static int CalculateMaxTagCost(TerrainDef terrain, Dictionary<string, PathCost> pathCostData)
		{
			var maxTagCost = PathCost.Default.cost;

			if (terrain.tags != null)
			{
				foreach (var tag in terrain.tags)
				{
					if (pathCostData.TryGetValue(tag, out var tagCost))
					{
						maxTagCost = Math.Max(maxTagCost, tagCost.cost);
					}
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
		/// <param name="terrainPathCost">Vanilla path cost.</param>
		/// <returns>Cost to use for pathing.</returns>
		private static int CalculatePathCost(int maxTagCost, Traversability passability, PathCost defaultCost,
			int terrainPathCost)
		{
			if (maxTagCost > PathCost.Default.cost)
			{
				return maxTagCost;
			}
			if (passability == Traversability.Impassable)
			{
				return PathCost.Impassable.cost;
			}

			if (defaultCost == PathCost.Default)
			{
				return terrainPathCost;
			}

			return defaultCost.cost;
		}

		/// <summary>
		/// Precalculates pathCosts for each terrain of each movement type.
		/// </summary>
		private static void SetMovementPathCosts()
		{
			var movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			var terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			var movementCount = movementDefs.Count;
			_terrainCount = terrainDefs.Count;
			_costs = new int[movementDefs.Count * terrainDefs.Count];

			for (var movementIndex = 0; movementIndex < movementCount; ++movementIndex)
			{
				var movementDef = movementDefs[movementIndex];
				var defaultCost = movementDef.defaultCost;

				for (var terrainIndex = 0; terrainIndex < _terrainCount; ++terrainIndex)
				{
					var terrainDef = terrainDefs[terrainIndex];
					var maxTagCost = CalculateMaxTagCost(terrainDef, movementDef.tagCosts.data);
					_costs[movementIndex * _terrainCount + terrainIndex] = CalculatePathCost(maxTagCost, terrainDef.passability,
						defaultCost, terrainDef.pathCost);
				}
			}
		}

		/// <summary>
		/// Movement path cost initialization.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				SetMovementPathCosts();
				Report.Debug("Movement type initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Movement type initialization failed:");
				Report.Error($"{exception.ToString()}");
			}
		}

		public static int Get(int movementIndex, int terrainIndex)
		{
			return _costs[movementIndex * _terrainCount + terrainIndex];
		}

	}
}