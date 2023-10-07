using System;
using System.Collections.Generic;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// This class contains pawn movement aware versions of vanilla methods used for finding valid locations.
	/// These are then replaced explicitly by the Harmony patches in Patches/LocationChoosing.
	/// Known problematic functions include:
	/// GenGrid.Standable
	/// GenGrid.Walkable
	/// GenGrid.WalkableByNormal
	/// </summary>
	public static class LocationFinding
	{
		/// <summary>
		/// Checks if pawns with a specific movement type should be standing on a certain cell.
		/// </summary>
		/// <param name="movementDef">Movement type to check.</param>
		/// <param name="map">Current map.</param>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the movement allows standing here.</returns>
		public static bool CanStandAt(MovementDef movementDef, Map map, IntVec3 cell)
		{
			TerrainDef terrainDef = cell.GetTerrain(map);
			if (terrainDef == null || !movementDef.CanEnterTerrain(terrainDef))
			{
				return false;
			}

			List<Thing> thingList = map.thingGrid.ThingsListAt(cell);
			for (int index = 0; index < thingList.Count; ++index)
			{
				if (thingList[index].def.passability != Traversability.Standable)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// A pawn can spawn in a cell if it is in the border, there is a path to the colony, and the movement type allows
		/// standing up in the cell.
		/// </summary>
		/// <param name="movementDef">Movement type to check.</param>
		/// <param name="map">Current map.</param>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the cell is a good spawning point for this movement type.</returns>
		public static bool CanSpawnAt(MovementDef movementDef, Map map, IntVec3 cell)
		{
			return LocationFinding.CanStandAt(movementDef, map, cell) && cell.GetDistrict(map).TouchesMapEdge &&
			       map.reachability.CanReachColony(cell);
		}

		/// <summary>
		/// Pawn movement aware version of CellFinder.TryRandomClosewalkCellNear.
		/// </summary>
		/// <param name="pawn">Pawn making the request.</param>
		/// <param name="radius">Maximum lookup radious around the pawn position.</param>
		/// <param name="result">Found position if any.</param>
		/// <param name="extraValidator">Extra condition that the found position must meet.</param>
		/// <returns>True if a location was found.</returns>
		public static bool TryRandomClosewalkCellNear(Pawn pawn, int radius, out IntVec3 result,
			Predicate<IntVec3> extraValidator = null)
		{
			return CellFinder.TryFindRandomReachableCellNear(pawn.Position, pawn.Map, radius,
				TraverseParms.For(pawn, Danger.Deadly, TraverseMode.NoPassClosedDoors).WithFenceblocked(true), c =>
				{
					if (!pawn.MovementContext().CanStandAt(c))
					{
						return false;
					}

					return extraValidator == null || extraValidator(c);
				}, null, out result);
		}

		/// <summary>
		/// Search for a valid entry cell for a pawn with a specific movement type.
		/// </summary>
		/// <param name="result">Found cell.</param>
		/// <param name="map">Current map</param>
		/// <param name="roadChance">Probability of spawning on a road.</param>
		/// <param name="allowFogged">Allow spawning in fogged tiles.</param>
		/// <param name="extraValidator">Optional extra validation.</param>
		/// <param name="movementDef">Movement definition of the pawn.</param>
		/// <returns>True if a cell was found.</returns>
		public static bool TryFindRandomPawnEntryCell(out IntVec3 result, Map map, float roadChance,
			bool allowFogged, Predicate<IntVec3> extraValidator, MovementDef movementDef)
		{
			return CellFinder.TryFindRandomEdgeCellWith( (cell =>
			{
				bool foggedPrevents = !allowFogged && cell.Fogged(map);
				if (foggedPrevents || !CanSpawnAt(movementDef, map, cell))
				{
					return false;
				}
				return extraValidator == null || extraValidator(cell);
			}), map, roadChance, out result);
		}
	}
}