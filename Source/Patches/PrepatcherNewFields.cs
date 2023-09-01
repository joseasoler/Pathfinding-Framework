using PathfindingFramework.MapPathCosts;
using Prepatcher;
using Verse;

namespace PathfindingFramework.Patches
{
	/// <summary>
	/// Defines new fields using prepatcher.
	/// </summary>
	public static class PrepatcherNewFields
	{
		private const string NoPrepatcher = "Fatal error: Prepatcher is required!";

		/// <summary>
		/// Returned when Prepatcher is not found.
		/// </summary>
		private static MovementDef _noPrepatcherMovementDef = null;

		/// <summary>
		/// Returned when Prepatcher is not found.
		/// </summary>
		private static MapPathCostGrid _noPrepatcherMapPathCostGrid = null;

		[PrepatcherField]
		public static ref MovementDef MovementDef(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementDef;
		}

		[PrepatcherField]
		public static ref MapPathCostGrid MapPathCostGrid(this Map map)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMapPathCostGrid;
		}
	}
}