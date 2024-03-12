using RimWorld;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Job definitions used by the C# code of Pathfinding Framework.
	/// </summary>

	[DefOf]
	public static class JobDefOf
	{
		/// <summary>
		/// Force pawns in unsafe terrain to seek safer terrain.
		/// </summary>
		public static JobDef PF_Job_SeekSafeTerrain;
	}
}