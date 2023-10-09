using RimWorld;

namespace PathfindingFramework
{
	/// <summary>
	/// Movement type definitions used in the code of the mod.
	/// </summary>
	[DefOf]
	public static class MovementDefOf
	{
		/// <summary>
		/// Movement type used by default. Should correspond to vanilla path costs.
		/// </summary>
		public static MovementDef PF_Movement_Terrestrial;
	}
}