using RimWorld;

namespace PathfindingFramework
{
	/// <summary>
	/// Movement definitions used by the C# code of Pathfinding Framework.
	/// </summary>
	[DefOf]
	public static class MovementDefOf
	{
		/// <summary>
		/// Movement type intended for exclusively aquatic creatures.
		/// </summary>
		public static MovementDef PF_Movement_Aquatic;

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