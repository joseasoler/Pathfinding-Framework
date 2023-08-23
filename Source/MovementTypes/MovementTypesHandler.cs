using System;
using System.Collections.Generic;
using System.Text;
using PathfindingFramework.Def;
using Verse;

namespace PathfindingFramework.MovementTypes
{
	/// <summary>
	/// Parses movement types from XML and stores them in memory, allowing faster access and usage.
	/// This class relies on Defs having an index field that starts at zero. 
	/// </summary>
	public class MovementTypesHandler
	{
		/// <summary>
		/// List of movement types and their path costs. Accessed by [MovementTypeDef.index][TerrainDef.index].
		/// </summary>
		private static List<List<int>> PathCostsByMovementTypeIndex;

		/// <summary>
		/// Obtain the largest pathing cost of the tags of a specific terrain.  
		/// </summary>
		/// <param name="terrain">Terrain being evaluated</param>
		/// <param name="pathCostData">Pathing cost of each tag for a certain movement type.</param>
		/// <returns>Default if no tag was found, the maximum pathing cost otherwise.</returns>
		private static int GetMaxTagCost(TerrainDef terrain, Dictionary<string, PathCost> pathCostData)
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
		private static int GetPathCost(int maxTagCost, Traversability passability, PathCost defaultCost,
			int terrainPathCost)
		{
			if (maxTagCost > PathCost.Default.cost)
			{
				return maxTagCost;
			}
			else if (passability == Traversability.Impassable)
			{
				return PathCost.Impassable.cost;
			}

			return (defaultCost == PathCost.Default) ? defaultCost.cost : terrainPathCost;
		}

		/// <summary>
		/// Precalculates pathCosts for each terrain of each movement type.
		/// </summary>
		private static void SetMovementPathCosts()
		{
			PathCostsByMovementTypeIndex = new List<List<int>>();

			foreach (var movementType in DefDatabase<MovementTypeDef>.AllDefsListForReading)
			{
				PathCostsByMovementTypeIndex.Add(new List<int>());

				PathCostsByMovementTypeIndex[PathCostsByMovementTypeIndex.Count - 1] = new List<int>();
				var pathCosts = PathCostsByMovementTypeIndex[PathCostsByMovementTypeIndex.Count - 1];
				var defaultCost = movementType.defaultCost;

				foreach (var terrain in DefDatabase<TerrainDef>.AllDefsListForReading)
				{
					var maxTagCost = GetMaxTagCost(terrain, movementType.tagCosts.data);
					pathCosts.Add(GetPathCost(maxTagCost, terrain.passability, defaultCost, terrain.pathCost));
				}
			}
		}

		/// <summary>
		/// Movement type initialization.
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

		/// <summary>
		/// Append a movement types report to the game log.
		/// </summary>
		public static void ShowReport()
		{
			StringBuilder sb = new StringBuilder("Movement types report:\n");
			foreach (var movementType in DefDatabase<MovementTypeDef>.AllDefsListForReading)
			{
				sb.AppendLine($"{movementType.defName}:");
				foreach (var terrain in DefDatabase<TerrainDef>.AllDefsListForReading)
				{
					sb.AppendLine($"\t{terrain}: {PathCostsByMovementTypeIndex[movementType.index][terrain.index]}");
				}
			}

			Report.Notice(sb.ToString());
		}
	}
}