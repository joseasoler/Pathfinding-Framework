using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.DevTool
{
	public static class MovementCostDebugOutput
	{
		[DebugOutput(category: PathfindingFramework.Name, onlyWhenPlaying: false)]
		public static void MovementCosts()
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			int movementCount = movementDefs.Count;
			int terrainCount = terrainDefs.Count;

			var dataTable = new string[movementCount + 1, terrainCount + 1];
			dataTable[0, 0] = "Movement type";

			for (var movementIndex = 0; movementIndex < movementCount; ++movementIndex)
			{
				MovementDef movementDef = movementDefs[movementIndex];
				dataTable[movementIndex + 1, 0] = movementDef.label;

				for (var terrainIndex = 0; terrainIndex < terrainCount; ++terrainIndex)
				{
					TerrainDef terrainDef = terrainDefs[terrainIndex];
					dataTable[0, terrainIndex + 1] = terrainDef.label;
					dataTable[movementIndex + 1, terrainIndex + 1] = movementDef.PathCosts[terrainIndex].ToString();
				}
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}