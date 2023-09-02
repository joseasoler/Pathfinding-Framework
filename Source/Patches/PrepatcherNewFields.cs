using PathfindingFramework.MapPathCosts;
using PathfindingFramework.MovementContexts;
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

		private static MovementDef _noPrepatcherMovementDef = null;
		private static MapPathCostGrid _noPrepatcherMapPathCostGrid = null;
		private static MovementContext _noPrepatcherMovementContext = null;
		private static MovementContextData _noPrepatcherMovementContextData = null;

		[PrepatcherField]
		public static ref MovementDef MovementDef(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementDef;
		}

		[PrepatcherField]
		public static ref MovementContext MovementContext(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementContext;
		}

		[PrepatcherField]
		public static ref MapPathCostGrid MapPathCostGrid(this Map map)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMapPathCostGrid;
		}


		[PrepatcherField]
		public static ref MovementContextData MovementContextData(this Map map)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementContextData;
		}
	}
}