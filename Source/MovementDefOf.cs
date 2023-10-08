using RimWorld;

namespace PathfindingFramework
{
	[DefOf]
	public static class MovementDefOf
	{
		/// <summary>
		/// Movement type used by default. Should correspond to vanilla path costs.
		/// </summary>
		public static MovementDef PF_Movement_Terrestrial;

		/// <summary>
		/// Aquatic creatures can swim through deep water with ease, but are helpless on land.
		/// </summary>
		public static MovementDef PF_Movement_Aquatic;
	}
}