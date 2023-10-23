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
		
		/// <summary>
		/// Granted temporarily when a terrestrial creature finds itself in water unexpectedly in specific cases. Currently
		/// this only happens with Giddy-Up 2.
		/// </summary>
		public static MovementDef PF_Movement_Terrestrial_Unsafe;
	}
}