using PathfindingFramework.MapComponents.MapPathCosts;
using PathfindingFramework.MapComponents.MovementContexts;
using PathfindingFramework.PawnGraphic;
using Prepatcher;
using RimWorld;
using Verse;

namespace PathfindingFramework.ExtensionMethodCaches
{
	public static class Methods
	{
		private static readonly RefDictionary<MovementDef> _thingDefMovementDefCache = new();

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="thingDef">Apparel or pawn race being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this ThingDef thingDef)
		{
			return ref _thingDefMovementDefCache.Get(thingDef.shortHash);
		}

		private static readonly RefDictionary<MovementDef> _lifeStageDefMovementDefCache = new();

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="lifeStageDef">Life stage being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this LifeStageDef lifeStageDef)
		{
			return ref _lifeStageDefMovementDefCache.Get(lifeStageDef.shortHash);
		}

		private static readonly RefDictionary<MovementDef> _geneDefMovementDefCache = new();

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="geneDef">Gene being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this GeneDef geneDef)
		{
			return ref _geneDefMovementDefCache.Get(geneDef.shortHash);
		}

		private static readonly RefDictionary<MovementDef> _hediffDefMovementDefCache = new();

		/// <summary>
		/// Stores the movement type granted by the MovementExtensions of this def.
		/// This is initialized after the game loads, and never modified afterwards.
		/// </summary>
		/// <param name="hediffDef">Health difference being checked.</param>
		/// <returns>Movement related to this def.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this HediffDef hediffDef)
		{
			return ref _hediffDefMovementDefCache.Get(hediffDef.shortHash);
		}

		private static readonly RefDictionary<MovementDef> _pawnMovementDefCache = new();

		/// <summary>
		/// Movement definition currently in use for this pawn.
		/// Kept up to date by PawnMovementUpdater.Update.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Movement used by this pawn.</returns>
		[PrepatcherField]
		public static ref MovementDef MovementDef(this Pawn pawn)
		{
			return ref _pawnMovementDefCache.Get(pawn.thingIDNumber);
		}

		private static readonly RefDictionary<MovementContext> _pawnMovementContextCache = new();

		/// <summary>
		/// Movement context data of the current pawn.
		/// Kept updated by MovementContextData.UpdatePawn.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>Movement context of the pawn.</returns>
		[PrepatcherField]
		public static ref MovementContext MovementContext(this Pawn pawn)
		{
			return ref _pawnMovementContextCache.Get(pawn.thingIDNumber);
		}

		private static readonly RefDictionary<GraphicContext> _pawnGraphicContextCache = new();

		/// <summary>
		/// Graphic context of this pawn, if any.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Graphic context of this pawn.</returns>
		[PrepatcherField]
		public static ref GraphicContext GraphicContext(this Pawn pawn)
		{
			return ref _pawnGraphicContextCache.Get(pawn.thingIDNumber);
		}

		private static readonly RefDictionary<TerrainDef> _pawnCurrentTerrainDefCache = new();

		/// <summary>
		/// Terrain on which this pawn is currently standing on.
		/// </summary>
		/// <param name="pawn">Pawn being checked.</param>
		/// <returns>Locomotion extension of this pawn.</returns>
		[PrepatcherField]
		public static ref TerrainDef CurrentTerrainDef(this Pawn pawn)
		{
			return ref _pawnCurrentTerrainDefCache.Get(pawn.thingIDNumber);
		}

		private static readonly RefDictionary<MapPathCostGrid> _mapPathCostGridCache = new();

		/// <summary>
		/// Map path cost grid of a map.
		/// Multiple harmony patches are responsible for keeping this path cost grid up to date.
		/// </summary>
		/// <param name="map">Map being checked.</param>
		/// <returns>Path cost grid for this map.</returns>
		[PrepatcherField]
		public static ref MapPathCostGrid MapPathCostGrid(this Map map)
		{
			return ref _mapPathCostGridCache.Get(map.uniqueID);
		}

		private static readonly RefDictionary<MovementContextData> _mapMovementContextDataCache = new();

		/// <summary>
		/// Movement context data for all pawns of a map.
		/// Used by MapPathCostGrid to trigger movement context updates.
		/// </summary>
		/// <param name="map">Map being checked.</param>
		/// <returns>Reference to the full movement context data.</returns>
		[PrepatcherField]
		public static ref MovementContextData MovementContextData(this Map map)
		{
			return ref _mapMovementContextDataCache.Get(map.uniqueID);
		}

		private static readonly RefDictionary<TerrainDef> _regionUniqueTerrainDef = new();

		/// <summary>
		/// Certain terrains must belong to regions which only have that terrain type. This field has a value in those
		/// cases.
		/// </summary>
		/// <param name="region">Region being checked</param>
		/// <returns>Null for regions that do not need to follow this rule, the terrain otherwise.</returns>
		[PrepatcherField]
		public static ref TerrainDef UniqueTerrainDef(this Region region)
		{
			return ref _regionUniqueTerrainDef.Get(region.id);
		}

		private static readonly RefDictionary<int> _terrainDefMovementIndexCache = new();

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
			return ref _terrainDefMovementIndexCache.Get(terrainDef.shortHash);
		}

		private static readonly RefDictionary<int> _terrainDefExtendedRegionType = new();

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
			return ref _terrainDefExtendedRegionType.Get(terrainDef.shortHash);
		}
	}
}