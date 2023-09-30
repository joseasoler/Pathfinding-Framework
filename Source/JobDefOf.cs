using RimWorld;
using Verse;

namespace PathfindingFramework
{
	[DefOf]
	public class JobDefOf
	{
		/// <summary>
		/// Force pawns in tiles to be avoided to seek safer terrain.
		/// </summary>
		public static JobDef PF_Job_SeekSafeTerrain;
	}
}