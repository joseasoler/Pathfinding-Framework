using System.Collections.Generic;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.TerrainDefInformation
{
	/// <summary>
	/// Initialize TerrainDef.MovementIndex. See PrepatcherNewFields for details.
	/// </summary>
	public static class TerrainMovementIndex
	{
		public static void Initialize()
		{
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;

			for (int terrainDefIndex = 0; terrainDefIndex < terrainDefs.Count; ++terrainDefIndex)
			{
				terrainDefs[terrainDefIndex].MovementIndex() = terrainDefIndex;
			}
		}
	}
}