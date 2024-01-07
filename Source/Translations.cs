using HarmonyLib;
using Verse;

namespace PathfindingFramework
{
	public static class Translations
	{
		public static string PF_BasePathCost;
		public static string PF_CellLabel;
		public static string PF_DebuggingTab;
		public static string PF_DebugLogHover;
		public static string PF_DebugLogLabel;
		public static string PF_District;
		public static string PF_DistrictCellCount;
		public static string PF_DistrictRegionCount;
		public static string PF_DistrictRegionType;
		public static string PF_DistrictRegionsMapEdge;
		public static string PF_FirePathCostLabel;
		public static string PF_GeneralTab;
		public static string PF_GrantedBy;
		public static string PF_GrantedByEndogene;
		public static string PF_GrantedByXenogene;
		public static string PF_GrantsMovementType;
		public static string PF_HasDoorLabel;
		public static string PF_HasFenceLabel;
		public static string PF_HasIgnoreRepeatersLabel;
		public static string PF_IgnoreFireHover;
		public static string PF_IgnoreFireLabel;
		public static string PF_IgnoreFireMovementLabel;
		public static string PF_InspectorHover;
		public static string PF_InspectorLabel;
		public static string PF_LogPathNotFoundHover;
		public static string PF_LogPathNotFoundLabel;
		public static string PF_LogRegionCalculatorErrorHover;
		public static string PF_LogRegionCalculatorErrorLabel;
		public static string PF_Movement;
		public static string PF_MovementCostsLabel;
		public static string PF_MovementReportText;
		public static string PF_NoFencesMovementLabel;
		public static string PF_NonIgnoreRepeatersPathCostLabel;
		public static string PF_PathCostsLabel;
		public static string PF_PawnMovementTab;
		public static string PF_PawnMovementWarningLabel;
		public static string PF_RegionExtentsClose;
		public static string PF_RegionExtentsLimit;
		public static string PF_RegionId;
		public static string PF_RegionLabel;
		public static string PF_RegionLinks;
		public static string PF_RegionNone;
		public static string PF_RegionType;
		public static string PF_ResetSettingsHover;
		public static string PF_ResetSettingsLabel;
		public static string PF_Room;
		public static string PF_RoomDistrictCount;
		public static string PF_RoomOutdoors;
		public static string PF_RoomProper;
		public static string PF_RoomRole;
		public static string PF_RoomRoleNone;
		public static string PF_TerrainCost;
		public static string PF_ThingsPathCostLabel;
		public static string PF_VanillaLabel;
		public static string PF_WildAnimalRelocationHover;
		public static string PF_WildAnimalRelocationLabel;

		public static string Apparel;
		public static string Genes;
		public static string Health;
		public static string Impassable;
		public static string No;
		public static string PawnsTabShort;
		public static string Snow_Label;
		public static string Terrain_Label;
		public static string Unknown;
		public static string Yes;

		public static void Initialize()
		{
			foreach (var field in typeof(Translations).GetFields(AccessTools.allDeclared))
			{
				if (field.FieldType != typeof(string))
				{
					continue;
				}

				field.SetValue(null, (string) field.Name.Translate());
			}

			Report.Debug("Translations initialization complete.");
		}
	}
}