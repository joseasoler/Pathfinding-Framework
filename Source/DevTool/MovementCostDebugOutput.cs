using System.Collections.Generic;
using PathfindingFramework.Cache;
using Verse;

namespace PathfindingFramework.Debug
{
	public static class MovementCostDebugOutput
	{
		[DebugOutput(category: Mod.Name, onlyWhenPlaying: false)]
		public static void MovementCosts()
		{
			var movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			var terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;
			var movementCount = movementDefs.Count;
			var terrainCount = terrainDefs.Count;

			string[,] dataTable = new string[movementCount + 1, terrainCount + 1];
			dataTable[0, 0] = "Movement type";

			for (var movementIndex = 0; movementIndex < movementCount; ++movementIndex)
			{
				var movementDef = movementDefs[movementIndex];
				dataTable[movementIndex + 1, 0] = movementDef.label;

				for (var terrainIndex = 0; terrainIndex < terrainCount; ++terrainIndex)
				{
					var terrainDef = terrainDefs[terrainIndex];
					dataTable[0, terrainIndex + 1] = terrainDef.label;
					dataTable[movementIndex + 1, terrainIndex + 1] =
						MovementPathCostCache.Get(movementIndex, terrainIndex).ToString();
				}
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}