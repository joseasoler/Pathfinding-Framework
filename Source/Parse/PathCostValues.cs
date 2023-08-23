namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Defines specific pathfinding cost values which can be referenced by name.
	/// </summary>
	public enum PathCostValues
	{
		/// <summary>
		/// Use the cost defined by the TerrainDef itself.
		/// </summary>
		Default = -1,

		/// <summary>
		/// Terrain that should be avoided, but the pawn can still move extremely slowly over it.
		/// </summary>
		Avoid = 2000,

		/// <summary>
		/// Impassable terrain. Marking too many terrains as impassable might result in spawning issues and other problems.
		/// </summary>
		Impassable = 10000,


		/// <summary>
		/// PathCost value not correctly defined in XML files.
		/// </summary>
		Invalid = 20000,
	}
}