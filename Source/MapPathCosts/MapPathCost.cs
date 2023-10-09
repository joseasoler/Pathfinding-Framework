using System.Runtime.InteropServices;

namespace PathfindingFramework.MapPathCosts
{
	/// <summary>
	/// Stores common path costs of a specific cell of the map. These are shared between different movement contexts.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MapPathCost
	{
		/// <summary>
		/// Keeps track of pathfinding costs associated with fire presence.
		/// In vanilla, Verse.AI.PathGrid.CalculatedCostAt calculates this cost when perceivedStatic is set to true.
		/// In the Pathfinding Framework, a precalculated copy of this information is used to calculate all path grids.
		/// PF ignores costs from fires attached to pawns to improve performance.
		/// </summary>
		public short fire;

		/// <summary>
		/// Path cost of things contained in the cell.
		/// Follows the same logic as Verse.AI.PathGrid.CalculatedCostAt for generating the pathCost in the first
		/// ThingsListAt loop.
		/// </summary>
		public short things;

		/// <summary>
		/// Path cost of things with IsPathCostIgnoreRepeater returning false in the cell.
		/// Follows the same logic as Verse.AI.PathGrid.CalculatedCostAt for generating the pathCost in the first
		/// ThingsListAt loop.
		/// </summary>
		public short nonIgnoreRepeaterThings;

		/// <summary>
		/// Keeps track of the path cost associated with snow.
		/// </summary>
		public sbyte snow;

		/// <summary>
		/// True if the cell contains one or more things with IsPathCostIgnoreRepeater returning true.
		/// </summary>
		public bool hasIgnoreRepeater;

		/// <summary>
		/// True for cells containing a door.
		/// Used to replicate Verse.AI.PathGrid.CalculatedCostAt logic to increase path costs if both the previous and
		/// current tiles have doors.
		/// </summary>
		public bool hasDoor;

		/// <summary>
		/// Keeps track of fence presence.
		/// In vanilla, Verse.AI.PathGrid.CalculatedCostAt considers fences as impassable in the fenceBlocked pathing.
		/// </summary>
		public bool hasFence;
	}
}