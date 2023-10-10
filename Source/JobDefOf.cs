using RimWorld;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Job definitions used in the code of the mod.
	/// </summary>
	[DefOf]
	public class JobDefOf
	{
		/// <summary>
		/// Force pawns in unsafe terrain to seek safer terrain.
		/// </summary>
		public static JobDef PF_Job_SeekSafeTerrain;
	}
}