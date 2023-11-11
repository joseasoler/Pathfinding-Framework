using PathfindingFramework.MapPathCosts;
using PathfindingFramework.MovementContexts;
using PathfindingFramework.PawnGraphic;
using Prepatcher;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches
{
	/// <summary>
	/// Defines new fields for some game entities using Prepatcher.
	/// </summary>
	public static class PrepatcherNewFields
	{
		private static MovementDef _noPrepatcherMovementDef = null;
		private static MapPathCostGrid _noPrepatcherMapPathCostGrid = null;
		private static MovementContext _noPrepatcherMovementContext = null;
		private static MovementContextData _noPrepatcherMovementContextData = null;
		private static GraphicContext _noPrepatcherGraphicContext = null;
		private static TerrainDef _noPrepatcherTerrainDef = null;
		private static int _noPrepatcherInt = 0;

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="thingDef">Apparel or pawn race being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this ThingDef thingDef)
		{
			return ref _noPrepatcherMovementDef;
		}

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="lifeStageDef">Life stage being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this LifeStageDef lifeStageDef)
		{
			return ref _noPrepatcherMovementDef;
		}

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="geneDef">Gene being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this GeneDef geneDef)
		{
			return ref _noPrepatcherMovementDef;
		}

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="hediffDef">Health difference being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this HediffDef hediffDef)
		{
			return ref _noPrepatcherMovementDef;
		}

		/// <summary>
		/// Movement definition currently in use for this pawn.
		/// Kept up to date by PawnMovementUpdater.Update.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Movement used by this pawn.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this Pawn pawn)
		{
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
			return ref _noPrepatcherMovementContext;
		}

		/// <summary>
		/// Graphic context of this pawn, if any.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Graphic context of this pawn.</returns>
		[PrepatcherField]
		public static ref GraphicContext GraphicContext(this Pawn pawn)
		{
			return ref _noPrepatcherGraphicContext;
		}

		/// <summary>
		/// Terrain on which this pawn is currently standing on.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Locomotion extension of this pawn.</returns>
		[PrepatcherField]
		public static ref TerrainDef CurrentTerrainDef(this Pawn pawn)
		{
			return ref _noPrepatcherTerrainDef;
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
			return ref _noPrepatcherMovementContextData;
		}

		/// <summary>
		/// Certain terrains must belong to regions which only have that terrain type. This field has a value in those
		/// cases.
		/// </summary>
		/// <param name="region">Region being checked</param>
		/// <returns>Null for regions that do not need to follow this rule, the terrain otherwise.</returns>
		[PrepatcherField]
		public static ref TerrainDef UniqueTerrainDef(this Region region)
		{
			return ref _noPrepatcherTerrainDef;
		}

		/// <summary>
		/// Index of this TerrainDef in the array of path costs of MovementDef instances.
		/// Originally the plan was to use TerrainDef.index. Unfortunately this value is not valid for Defs that inherit
		/// from TerrainDef, such as the ActiveTerrains from Biomes! Core or Vanilla Expanded Framework.
		/// </summary>
		/// <param name="terrainDef">Terrain definition being checked.</param>
		/// <returns>Value equal or large to zero, and unique for all TerrainDefs. Corresponds to its position in
		/// DefDatabase.AllDefsListForReading.
		/// </returns>
		[PrepatcherField]
		public static ref int MovementIndex(this TerrainDef terrainDef)
		{
			return ref _noPrepatcherInt;
		}

		/// <summary>
		/// Certain terrains must only sare regions with cells having the same terrain. This is identified by this field
		/// having a value larger than zero.
		/// Impassable terrains with values larger than zero have been made passable by one of the movement types.
		/// See TerrainRegionType for details.
		/// </summary>
		/// <param name="terrainDef">Terrain being checked.</param>
		/// <returns>Zero if vanilla region formation is allowed for this terrain. A number larger than zero if the
		/// terrain should only be grouped with terrains with the same value.
		/// </returns>
		[PrepatcherField]
		public static ref int ExtendedRegionType(this TerrainDef terrainDef)
		{
			return ref _noPrepatcherInt;
		}
	}
}