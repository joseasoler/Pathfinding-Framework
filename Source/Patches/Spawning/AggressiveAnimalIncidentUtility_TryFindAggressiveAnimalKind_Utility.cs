using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using RimWorld.Planet;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Remove animals with MovementDef.manhuntersRequireWater enabled from maps lacking access to water.
	/// </summary>
	public static class AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Utility
	{
		private static bool HasAccessToWater(int tileID)
		{
			WorldGrid grid = Find.WorldGrid;
			if (tileID == -1 || !grid.InBounds(tileID))
			{
				return false;
			}

			Tile tile = grid[tileID];
			if (tile.WaterCovered || !tile.Rivers.NullOrEmpty())
			{
				return true;
			}

			List<int> outNeighbors = new List<int>();
			grid.GetTileNeighbors(tileID, outNeighbors);
			foreach (int currentTileID in outNeighbors)
			{
				if (grid[currentTileID].WaterCovered)
				{
					return true;
				}
			}

			return false;
		}

		public static List<PawnKindDef> RemoveWaterAnimalsIfNeeded(List<PawnKindDef> allAggressiveAnimals,
			int tileID)
		{
			bool accessToWater = HasAccessToWater(tileID);
			List<PawnKindDef> result = new List<PawnKindDef>();
			foreach (PawnKindDef animalDef in allAggressiveAnimals)
			{
				MovementDef movementDef = animalDef.race.MovementDef();
				if (accessToWater || movementDef == null || !movementDef.manhuntersRequireWater)
				{
					result.Add(animalDef);
				}
			}

			return result;
		}
	}
}