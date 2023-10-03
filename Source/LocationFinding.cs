using System;
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
	}
}