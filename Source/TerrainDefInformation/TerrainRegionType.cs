using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.TerrainDefInformation
{
	/// <summary>
	/// In vanilla, region types are determined as follows.
	/// * All impassable terrains are grouped into RegionType.ImpassableFreeAirExchange.
	/// * Passable terrains are grouped together depending on their Door/Fence/Normal status.
	/// In Pathfinding Framework, there are two additional cases which we must check.
	/// * The first one is impassable terrains which are passable for one or more movement types.
	/// * The second case is a movement type which defines a defaultCost of Unsafe.
	///
	/// In the first case, these impassable terrains are defined as regions of type Normal, and grouped together only with
	/// cells containing the same terrain type.
	///
	/// In the second case, terrains having one of the terrain tags that the movement can traverse, must also be grouped
	/// together only with cells containing the same terrain type.
	///
	/// The end goal of all this is to ensure that the check made in Region_Allows_Patch always yields the correct result,
	/// regardless of the cell chosen for terrain checks.
	/// </summary>
	public static class TerrainRegionType
	{
		public static void Initialize()
		{
			const int extendedRegionTypeOffset = (int)RegionType.Set_All * 2;
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			List<TerrainDef> terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;

			for (int terrainDefIndex = 0; terrainDefIndex < terrainDefs.Count; ++terrainDefIndex)
			{
				TerrainDef terrainDef = terrainDefs[terrainDefIndex];
				terrainDef.ExtendedRegionType() = 0;
				for (int movementDefIndex = 0; movementDefIndex < movementDefs.Count; ++movementDefIndex)
				{
					MovementDef movementDef = movementDefs[movementDefIndex];
					if ((terrainDef.passability == Traversability.Impassable || // First case
						    movementDef.defaultCost == PathCost.Unsafe) && // Second case
					    movementDef.CanEnterTerrain(terrainDef))
					{
						terrainDef.ExtendedRegionType() = extendedRegionTypeOffset + terrainDef.MovementIndex();
						break;
					}
				}
			}
		}
	}
}