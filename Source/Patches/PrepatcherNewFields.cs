using PathfindingFramework.MapPathCosts;
using PathfindingFramework.MovementContexts;
using Prepatcher;
using Verse;

namespace PathfindingFramework.Patches
{
	/// <summary>
	/// Defines new fields for some game entities using Prepatcher.
	/// </summary>
	public static class PrepatcherNewFields
	{
		private const string NoPrepatcher = "Fatal error: Prepatcher is required!";

		private static MovementDef _noPrepatcherMovementDef = null;
		private static MapPathCostGrid _noPrepatcherMapPathCostGrid = null;
		private static MovementContext _noPrepatcherMovementContext = null;
		private static MovementContextData _noPrepatcherMovementContextData = null;
		private static LocomotionExtension _noPrepatcherLocomotionExtension = null;
		private static TerrainDef _noPrepatcherTerrainDef = null;
		private static TerrainTagGraphicExtension _noPrepatcherTerrainTagGraphicExtension = null;
		private static bool _noPrepatcherBool = false;

		/// <summary>
		/// Movement definition currently in use for this pawn.
		/// Kept up to date by PawnMovementUpdater.Update.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Movement used by this pawn.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementDef;
		}

		/// <summary>
		/// Movement context data of the current pawn.
		/// Kept updated by MovementContextData.UpdatePawn.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>Movement context of the pawn.</returns>
		[PrepatcherField]
		public static ref MovementContext MovementContext(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementContext;
		}

		/// <summary>
		/// Locomotion extension of this pawn, if any.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Locomotion extension of this pawn.</returns>
		[PrepatcherField]
		public static ref LocomotionExtension LocomotionExtension(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherLocomotionExtension;
		}

		/// <summary>
		/// Terrain on which this pawn is currently standing on.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Locomotion extension of this pawn.</returns>
		[PrepatcherField]
		public static ref TerrainDef CurrentTerrainDef(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherTerrainDef;
		}

		/// <summary>
		/// Terrain tag graphic extension of this pawn, if any.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Terrain tag graphic extension of this pawn.</returns>
		[PrepatcherField]
		public static ref TerrainTagGraphicExtension TerrainTagGraphicExtension(this Pawn pawn)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherTerrainTagGraphicExtension;
		}

		/// <summary>
		/// Map path cost grid of a map.
		/// Multiple harmony patches are responsible for keeping this path cost grid up to date.
		/// </summary>
		/// <param name="map">Map being checked.</param>
		/// <returns>Path cost grid for this map.</returns>
		[PrepatcherField]
		public static ref MapPathCostGrid MapPathCostGrid(this Map map)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMapPathCostGrid;
		}

		/// <summary>
		/// Movement context data for all pawns of a map.
		/// Used by MapPathCostGrid to trigger movement context updates.
		/// </summary>
		/// <param name="map">Map being checked.</param>
		/// <returns>Reference to the full movement context data.</returns>
		[PrepatcherField]
		public static ref MovementContextData MovementContextData(this Map map)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherMovementContextData;
		}

		/// <summary>
		/// Regions with impassable terrains always contain a single TerrainDef. This field allows accessing it.
		/// </summary>
		/// <param name="region">Region being checked</param>
		/// <returns>Null for regions without impassable terrain, the terrain otherwise.</returns>
		[PrepatcherField]
		public static ref TerrainDef TerrainDef(this Region region)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherTerrainDef;
		}

		/// <summary>
		/// Impassable terrains with this value set to true can be traversed by one of the loaded movement definitions.
		/// This is set during game load by MovementDefUtils.PathCosts.Update.
		/// </summary>
		/// <param name="terrainDef">Terrain being checked.</param>
		/// <returns>True if it is impassable but some movement type can make it passable.</returns>
		[PrepatcherField]
		public static ref bool PassableWithAnyMovement(this TerrainDef terrainDef)
		{
			Report.ErrorOnce(NoPrepatcher);
			return ref _noPrepatcherBool;
		}
	}
}