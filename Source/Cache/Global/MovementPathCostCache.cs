using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using Verse;

namespace PathfindingFramework.Cache.Global
{
	/// <summary>
	/// Parses movement types from XML and stores them in memory in a format allowing faster access.
	/// The cache is initialized after game load, and never modified afterwards.
	/// </summary>
	public static class MovementPathCostCache
	{
		/// <summary>
		/// Stores a list of terrain path costs for each movement type.
		/// </summary>
		private static int[] _terrainPathCosts;

		/// <summary>
		/// Number of MovementDefs loaded in the game.
		/// </summary>
		private static int _movementCount;

		/// <summary>
		/// Number of TerrainDefs loaded in the game.
		/// </summary>
		private static int _terrainCount;

		/// <summary>
		/// Obtain the largest pathing cost of the tags of a specific terrain.
		/// </summary>
		/// <param name="terrainDef">Terrain being evaluated.</param>
		/// <param name="movementDef">Movement type being checked.</param>
		/// <returns>Default if no tag was found, the maximum pathing cost otherwise.</returns>
		private static int CalculateMaxTagCost(TerrainDef terrainDef, MovementDef movementDef)
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
			_movementCount = movementDefs.Count;
			_terrainCount = terrainDefs.Count;
			_terrainPathCosts = new int[movementDefs.Count * terrainDefs.Count];

			for (var movementIndex = 0; movementIndex < _movementCount; ++movementIndex)
			{
				var movementDef = movementDefs[movementIndex];
				var defaultCost = movementDef.defaultCost;

				for (var terrainIndex = 0; terrainIndex < _terrainCount; ++terrainIndex)
				{
					var terrainDef = terrainDefs[terrainIndex];
					var maxTagCost = CalculateMaxTagCost(terrainDef, movementDef);
					_terrainPathCosts[movementIndex * _terrainCount + terrainIndex] = CalculatePathCost(maxTagCost,
						terrainDef.passability,
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
				Report.Debug("Movement path cost cache initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Movement type initialization failed:");
				Report.Error($"{exception}");
			}
		}

		/// <summary>
		/// Obtain the path cost of a terrain for a specific movement.
		/// </summary>
		/// <param name="movementIndex">Index value of the MovementDef.</param>
		/// <param name="terrainIndex">Index value of the TerrainDef.</param>
		/// <returns>Path cost.</returns>
		public static int Get(int movementIndex, int terrainIndex)
		{
			return _terrainPathCosts[movementIndex * _terrainCount + terrainIndex];
		}

		/// <summary>
		/// Total number of MovementDefs available in the game.
		/// </summary>
		/// <returns>Integer with the number.</returns>
		public static int MovementCount()
		{
			return _movementCount;
		}

		public static List<MemoryUsageData> MemoryReport()
		{
			return new List<MemoryUsageData>
			{
				new MemoryUsageData(nameof(MovementPathCostCache), MemoryUsageData.Global, "Terrain path costs",
					MemoryUsageData.BytesFromArray(_terrainPathCosts))
			};
		}
	}
}