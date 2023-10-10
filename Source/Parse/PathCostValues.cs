using Verse.AI;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Defines specific pathfinding cost values which can be referenced by name.
	/// </summary>
	public enum PathCostValues : short
	{
		/// <summary>
		/// Use the cost defined by the TerrainDef itself.
		/// </summary>
		Default = -1,

		/// <summary>
		/// Terrain that the pawn should avoid. The pawn can still move extremely slowly over it.
		/// </summary>
		Avoid = PathGrid.ImpassableCost - 2,

		/// <summary>
		/// Terrain that is unsafe for the pawn. The pawn can still move extremely slowly over it.
		/// </summary>
		Unsafe = PathGrid.ImpassableCost - 1,

		/// <summary>
		/// Impassable terrain. Marking too many terrains as impassable might result in spawning issues and other problems.
		/// </summary>
		Impassable = PathGrid.ImpassableCost,


		/// <summary>
		/// PathCost value not correctly defined in XML files.
		/// </summary>
		Invalid = 20000,
	}
}