using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using RimWorld;
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
		private static bool ThingsAllowStandingAt(Map map, IntVec3 cell)
		{
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
		/// Checks if pawns with a specific movement type should be standing on a certain cell.
		/// </summary>
		/// <param name="movementDef">Movement type to check.</param>
		/// <param name="map">Current map.</param>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the movement allows standing here.</returns>
		public static bool CanStandAt(MovementDef movementDef, Map map, IntVec3 cell)
		{
			if (!cell.InBounds(map))
			{
				return false;
			}

			TerrainDef terrainDef = cell.GetTerrain(map);
			if (terrainDef == null || !movementDef.CanEnterTerrain(terrainDef))
			{
				return false;
			}

			return ThingsAllowStandingAt(map, cell);
		}

		/// <summary>
		/// Intended as a transpiler replacement for GenGrid.Standable.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <param name="_">Unused.</param>
		/// <param name="pawn">Pawn making the check.</param>
		/// <returns>True if the pawn can stand at the given location.</returns>
		public static bool CanPawnStandAt(IntVec3 cell, Map _, Pawn pawn)
		{
			return pawn.MovementContext().CanStandAt(cell);
		}

		/// <summary>
		/// A pawn can spawn in a cell if it is in the border, there is a path to the colony, and the movement type allows
		/// standing up in the cell.
		/// </summary>
		/// <param name="movementDef">Movement type to check.</param>
		/// <param name="map">Current map.</param>
		/// <param name="cell">Cell to check.</param>
		/// <returns>True if the cell is a good spawning point for this movement type.</returns>
		private static bool CanSpawnAt(MovementDef movementDef, Map map, IntVec3 cell)
		{
			return CanStandAt(movementDef, map, cell) && cell.GetDistrict(map).TouchesMapEdge &&
			       (movementDef.ignoreColonyReachability || map.reachability.CanReachColony(cell));
		}

		/// <summary>
		/// Pawn movement aware version of CellFinder.TryRandomClosewalkCellNear, specifying a position.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="pawn">Pawn making the request.</param>
		/// <param name="radius">Maximum lookup radius around the pawn position.</param>
		/// <param name="result">Found position if any.</param>
		/// <param name="extraValidator">Extra condition that the found position must meet.</param>
		/// <returns>True if a location was found.</returns>
		public static bool TryRandomClosewalkCellNearPos(IntVec3 position, Pawn pawn, int radius,
			out IntVec3 result, Predicate<IntVec3> extraValidator = null)
		{
			return CellFinder.TryFindRandomReachableNearbyCell(position, pawn.Map, radius,
				TraverseParms.For(pawn, Danger.Deadly, TraverseMode.NoPassClosedDoors).WithFenceblocked(true), c =>
				{
					if (!pawn.MovementContext().CanStandAt(c))
					{
						return false;
					}

					bool result = extraValidator == null || extraValidator(c);
					return result;
				}, null, out result);
		}

		/// <summary>
		/// Pawn movement aware version of CellFinder.TryRandomClosewalkCellNear.
		/// </summary>
		/// <param name="pawn">Pawn making the request.</param>
		/// <param name="radius">Maximum lookup radius around the pawn position.</param>
		/// <param name="result">Found position if any.</param>
		/// <param name="extraValidator">Extra condition that the found position must meet.</param>
		/// <returns>True if a location was found.</returns>
		public static bool TryRandomClosewalkCellNear(Pawn pawn, int radius, out IntVec3 result,
			Predicate<IntVec3> extraValidator = null)
		{
			return TryRandomClosewalkCellNearPos(pawn.Position, pawn, radius, out result, extraValidator);
		}

		/// <summary>
		/// Pawn movement aware version of CellFinder.RandomClosewalkCellNear.
		/// </summary>
		/// <param name="pawn">Pawn making the request.</param>
		/// <param name="radius">Maximum lookup radius around the pawn position.</param>
		/// <param name="extraValidator">Extra condition that the found position must meet.</param>
		/// <returns>Found location, or pawn location otherwise.</returns>
		public static IntVec3 RandomClosewalkCellNear(Pawn pawn, int radius, Predicate<IntVec3> extraValidator)
		{
			return TryRandomClosewalkCellNear(pawn, radius, out IntVec3 result, extraValidator) ? result : pawn.Position;
		}

		private static bool ValidRandomPawnEntryCell(IntVec3 cell, Map map, MovementDef movementDef, bool allowFogged,
			Predicate<IntVec3> extraValidator)
		{
			return (allowFogged || !cell.Fogged(map)) && CanSpawnAt(movementDef, map, cell) &&
			       (extraValidator == null || extraValidator(cell));
		}

		/// <summary>
		/// Search for a valid entry cell for a pawn with a specific movement type.
		/// </summary>
		/// <param name="foundCell">Found cell.</param>
		/// <param name="map">Current map</param>
		/// <param name="roadChance">Probability of spawning on a road.</param>
		/// <param name="allowFogged">Allow spawning in fogged tiles.</param>
		/// <param name="extraValidator">Optional extra validation.</param>
		/// <param name="pawnKindDef">Definition of the pawn.</param>
		/// <returns>True if a cell was found.</returns>
		public static bool TryFindRandomPawnEntryCell(out IntVec3 foundCell, Map map, float roadChance,
			bool allowFogged, Predicate<IntVec3> extraValidator, PawnKindDef pawnKindDef)
		{
			MovementDef movementDef = pawnKindDef.race?.MovementDef();
			if (movementDef != null)
			{
				return CellFinder.TryFindRandomEdgeCellWith((cell =>
						ValidRandomPawnEntryCell(cell, map, movementDef, allowFogged, extraValidator)), map, roadChance,
					out foundCell);
			}

			// Resort to vanilla search.
			return RCellFinder.TryFindRandomPawnEntryCell(out foundCell, map, roadChance, allowFogged, extraValidator);
		}

		/// <summary>
		/// Pawn aware version of CellFinder.StandableCellNear.
		/// </summary>
		/// <param name="cell">Destination cell.</param>
		/// <param name="map">Map of the cell.</param>
		/// <param name="radius">Search radius.</param>
		/// <param name="selectedPawns">List of pawns trying to find a standable cell.</param>
		/// <returns>Standable cell if any was found, otherwise an invalid cell.</returns>
		public static IntVec3 StandableCellNearForMovementTypes(IntVec3 cell, Map map, float radius,
			List<Pawn> selectedPawns)
		{
			List<MovementDef> movementDefs = new List<MovementDef>();

			for (int selectedPawnIndex = 0; selectedPawnIndex < selectedPawns.Count; ++selectedPawnIndex)
			{
				movementDefs.Add(selectedPawns[selectedPawnIndex].MovementDef());
			}

			int numCellsInRadius = GenRadial.NumCellsInRadius(radius);
			for (int cellIndex = 0; cellIndex < numCellsInRadius; ++cellIndex)
			{
				IntVec3 nearbyCell = GenRadial.RadialPattern[cellIndex] + cell;
				if (!ThingsAllowStandingAt(map, nearbyCell))
				{
					continue;
				}

				for (int movementDefIndex = 0; movementDefIndex < movementDefs.Count; ++movementDefIndex)
				{
					if (!movementDefs[movementDefIndex].CanEnterTerrain(nearbyCell.GetTerrain(map)))
					{
						nearbyCell = IntVec3.Invalid;
						break;
					}
				}

				if (nearbyCell != IntVec3.Invalid)
				{
					return nearbyCell;
				}
			}

			return IntVec3.Invalid;
		}

		/// <summary>
		/// Check if a region is passable. See Region_Allows_Patch for details.
		/// </summary>
		public static bool IsPassableRegion(Region region, MovementDef movementDef, Map map, bool isDestination,
			bool currentlyOnUnsafeTerrain)
		{
			TerrainDef regionTerrainDef = region.UniqueTerrainDef() ?? region.AnyCell.GetTerrain(map);

			// If the pawn is standing on unsafe terrain, allow traversing unsafe terrain regions if the destination is safe.
			short nonTraversablePathCost =
				isDestination || !currentlyOnUnsafeTerrain ? PathCost.Unsafe.cost : PathCost.Impassable.cost;
			short pathCost = movementDef.PathCosts[regionTerrainDef.MovementIndex()];
			return pathCost < nonTraversablePathCost;
		}

		/// <summary>
		/// Check if a pawn with the given movement type can reach the map edge from this position.
		/// </summary>
		/// <param name="movementDef">Movement type to check.</param>
		/// <param name="map">Current map.</param>
		/// <param name="cell">Starting position.</param>
		/// <returns></returns>
		public static bool CanReachMapEdge(MovementDef movementDef, Map map, IntVec3 cell)
		{
			Region region = cell.GetRegion(map);
			if (region == null)
			{
				return false;
			}

			if (region.District.TouchesMapEdge)
			{
				return true;
			}

			bool currentlyOnUnsafeTerrain = movementDef.PathCosts[cell.GetTerrain(map).index] == PathCost.Unsafe.cost;

			bool foundReg = false;

			RegionTraverser.BreadthFirstTraverse(region, EntryCondition, RegionProcessor, 9999);
			return foundReg;

			bool RegionProcessor(Region r)
			{
				foundReg = r.District.TouchesMapEdge;
				return foundReg;
			}

			bool EntryCondition(Region from, Region to)
			{
				return IsPassableRegion(to, movementDef, map, true, currentlyOnUnsafeTerrain);
			}
		}
	}
}