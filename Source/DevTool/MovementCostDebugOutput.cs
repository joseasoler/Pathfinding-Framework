using System.Collections.Generic;
using LudeonTK;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Generate a table with the path costs of every loaded movement type.
	/// </summary>
	public static class MovementCostDebugOutput
	{
		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: false)]
		public static void MovementCosts()
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			int movementCount = movementDefs.Count;
			int terrainCount = terrainDefs.Count;

			string[,] dataTable = new string[movementCount + 2, // Terrain, terrainindex, ...
				terrainCount + 1];
			dataTable[0, 0] = "TerrainDef";
			dataTable[1, 0] = "Terrain Index";

			for (int movementIndex = 0; movementIndex < movementCount; ++movementIndex)
			{
				MovementDef movementDef = movementDefs[movementIndex];
				dataTable[movementIndex + 2, 0] = movementDef.label;

				for (int terrainIndex = 0; terrainIndex < terrainCount; ++terrainIndex)
				{
					TerrainDef terrainDef = terrainDefs[terrainIndex];
					dataTable[0, terrainIndex + 1] = terrainDef.defName;
					dataTable[1, terrainIndex + 1] = terrainDef.MovementIndex().ToString();
					dataTable[movementIndex + 2, terrainIndex + 1] = new PathCost(movementDef.PathCosts[terrainIndex]).ToString();
				}
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}