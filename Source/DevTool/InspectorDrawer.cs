using System;
using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapPathCosts;
using PathfindingFramework.MovementContexts;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Shows a path cost inspector drawer when a mod setting is enabled and a hotkey is kept pressed.
	/// See Verse.CellInspectorDrawer for the vanilla implementation of the map inspector.
	/// </summary>
	public static class InspectorDrawer
	{
		private const float DistFromMouse = 26.0F;
		private const float LabelColumnWidth = 260.0F;
		private const float InfoColumnWidth = 160.0F;
		private const float WindowPadding = 12.0F;
		private const float ColumnPadding = 12.0F;
		private const float LineHeight = 24.0F;
		private const float WindowWidth = LabelColumnWidth + InfoColumnWidth + WindowPadding * 2 + ColumnPadding;
		private const float OutsideScreenOffset = 52.0F;

		/// <summary>
		/// True while Control+Shift+Q is held and the relevant mod setting is enabled.
		/// </summary>
		private static bool _pathCostInspectorActive;

		/// <summary>
		/// True while Control+Shift+E is held and the relevant mod setting is enabled.
		/// </summary>
		private static bool _movementContextInspectorActive;

		/// <summary>
		/// True while Control+Shift+R is held and the relevant mod setting is enabled.
		/// </summary>
		private static bool _regionInspectorActive;

		/// <summary>
		/// Current number of lines of the inspector.
		/// </summary>
		private static int _numLines;

		private static bool AnyInspectorActive()
		{
			return _pathCostInspectorActive || _movementContextInspectorActive || _regionInspectorActive;
		}

		public static void Update()
		{
			if (!Settings.Values.Inspectors || Current.ProgramState != ProgramState.Playing ||
			    WorldRendererUtility.WorldRenderedNow || Find.CurrentMap == null || !UI.MouseCell().InBounds(Find.CurrentMap))
			{
				_pathCostInspectorActive = false;
				_movementContextInspectorActive = false;
				_regionInspectorActive = false;
				return;
			}

			bool commonShortcutIsPressed = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
			                               (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

			if (!commonShortcutIsPressed)
			{
				return;
			}

			_pathCostInspectorActive = Input.GetKey(KeyCode.Q);
			_movementContextInspectorActive = !_pathCostInspectorActive && Input.GetKey(KeyCode.E);
			_regionInspectorActive = !_pathCostInspectorActive && !_movementContextInspectorActive && Input.GetKey(KeyCode.R);

			if (AnyInspectorActive())
			{
				GenUI.RenderMouseoverBracket();
			}
		}

		public static void OnGui()
		{
			if (!AnyInspectorActive() || Mouse.IsInputBlockedNow)
			{
				return;
			}

			Rect rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, WindowWidth,
				_numLines * LineHeight + LineHeight);

			_numLines = 0;
			rect.x += DistFromMouse;
			rect.y += DistFromMouse;
			if (rect.xMax > UI.screenWidth)
			{
				rect.x -= rect.width + OutsideScreenOffset;
			}

			if (rect.yMax > UI.screenHeight)
			{
				rect.y -= rect.height + OutsideScreenOffset;
			}

			Find.WindowStack.ImmediateWindow(1362348, rect, WindowLayer.Super, FillWindow);
		}

		private static string PathCostLabel(short pathCost)
		{
			return new PathCost(pathCost).ToString();
		}

		private static void FillWindow()
		{
			if (!AnyInspectorActive())
			{
				return;
			}

			IntVec3 cell = UI.MouseCell();
			Map map = Find.CurrentMap;
			if (!cell.InBounds(map))
			{
				return;
			}

			int cellIndex = map.cellIndices.CellToIndex(UI.MouseCell());
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(cellIndex);

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;

			string titleStr;
			if (_pathCostInspectorActive)
			{
				titleStr = Translations.PF_PathCostsLabel;
			}
			else if (_movementContextInspectorActive)
			{
				titleStr = Translations.PF_MovementCostsLabel;
			}
			else
			{
				titleStr = Translations.PF_RegionLabel;
			}

			DrawHeader(titleStr);
			// Draw shared initial properties.
			DrawRow(Translations.PF_CellLabel, $"{cell.x}, {cell.z} ({cellIndex})");

			DrawDivider();
			DrawRow(Translations.Terrain_Label, terrainDef.LabelCap);

			if (_pathCostInspectorActive)
			{
				FillPathCostInspectorWindow(map, cellIndex, terrainDef);
			}
			else if (_movementContextInspectorActive)
			{
				FillMovementContextInspectorWindow(map, cell);
			}
			else if (_regionInspectorActive)
			{
				FillRegionInspectorWindow(map, cell);
			}

			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private static void FillPathCostInspectorWindow(Map map, int cellIndex, TerrainDef terrainDef)
		{
			MapPathCostGrid mapPathCostGrid = map.MapPathCostGrid();
			MapPathCost mapPathCost = mapPathCostGrid.Get(cellIndex);

			// Terrain path cost calculation.
			List<string> movementTypeLabels = new List<string>();
			List<short> terrainPathCosts = new List<short>();
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			movementDefs.Remove(MovementDefOf.PF_Movement_Terrestrial_Unsafe);

			movementDefs.Sort((lhs, rhs) => lhs.priority.CompareTo(rhs.priority));

			for (int movementIndex = 0; movementIndex < movementDefs.Count; ++movementIndex)
			{
				MovementDef movementDef = movementDefs[movementIndex];

				string label = movementDef.LabelCap;
				movementTypeLabels.Add(label);
				short terrainPathCost = movementDef.PathCosts[terrainDef.MovementIndex()];
				terrainPathCosts.Add(terrainPathCost);
			}

			string basePathCostLabel = (terrainDef.passability != Traversability.Impassable)
				? terrainDef.pathCost.ToString()
				: Translations.Impassable;
			DrawRow(Translations.PF_BasePathCost, basePathCostLabel);
			for (int dataIndex = 0; dataIndex < movementTypeLabels.Count; ++dataIndex)
			{
				DrawRow(Translations.PF_TerrainCost.Formatted(movementTypeLabels[dataIndex]),
					PathCostLabel(terrainPathCosts[dataIndex]));
			}

			DrawDivider();
			DrawRow(Translations.Snow_Label, mapPathCost.snow.ToString());
			DrawRow(Translations.PF_FirePathCostLabel, mapPathCost.fire.ToString());
			DrawRow(Translations.PF_ThingsPathCostLabel, PathCostLabel(mapPathCost.things));
			DrawRow(Translations.PF_NonIgnoreRepeatersPathCostLabel, PathCostLabel(mapPathCost.nonIgnoreRepeaterThings));
			string hasIgnoreRepeaterStr = mapPathCost.hasIgnoreRepeater.ToStringYesNo();
			DrawRow(Translations.PF_HasIgnoreRepeatersLabel, hasIgnoreRepeaterStr);
			string hasDoorStr = mapPathCost.hasDoor.ToStringYesNo();
			DrawRow(Translations.PF_HasDoorLabel, hasDoorStr);
			string hasFenceStr = mapPathCost.hasFence.ToStringYesNo();
			DrawRow(Translations.PF_HasFenceLabel, hasFenceStr);
		}

		private static void FillMovementContextInspectorWindow(Map map, IntVec3 cell)
		{
			DrawDivider();
			short vanillaNormalCost = (short) map.pathing.Normal.pathGrid.PerceivedPathCostAt(cell);
			short vanillaFenceCost = (short) map.pathing.FenceBlocked.pathGrid.PerceivedPathCostAt(cell);
			DrawRow(Translations.PF_VanillaLabel, PathCostLabel(vanillaNormalCost));
			DrawRow(Translations.PF_NoFencesMovementLabel, PathCostLabel(vanillaFenceCost));

			DrawDivider();
			List<MovementContext> activeContexts = map.MovementContextData().ActiveContexts();
			activeContexts.Sort((contextLhs, contextRhs) =>
				contextLhs.MovementDef.priority.CompareTo(contextRhs.MovementDef.priority));

			foreach (MovementContext context in activeContexts)
			{
				short cost = (short) context.PathingContext.pathGrid.PerceivedPathCostAt(cell);
				string movementLabelCap = context.MovementDef.LabelCap;
				List<string> extraLabels = new List<string>();
				if (context.ShouldAvoidFences)
				{
					extraLabels.Add(Translations.PF_NoFencesMovementLabel);
				}

				if (context.CanIgnoreFire)
				{
					extraLabels.Add(Translations.PF_IgnoreFireMovementLabel);
				}

				string label = extraLabels.Count == 0
					? movementLabelCap
					: $"{movementLabelCap} ({string.Join(", ", extraLabels)})";
				DrawRow(label, PathCostLabel(cost));
			}
		}

		private static void FillRegionInspectorWindow(Map map, IntVec3 cell)
		{
			DrawDivider();
			Region region = map.regionGrid.GetValidRegionAt(cell);
			if (region == null)
			{
				DrawRow(Translations.PF_RegionType, Translations.PF_RegionNone);
				return;
			}

			string regionType = Enum.GetName(typeof(RegionType), region.type);

			DrawRow(Translations.PF_RegionId, region.id.ToString());
			DrawRow(Translations.PF_RegionType, regionType);
			DrawRow(Translations.PF_RegionLinks, region.links.Count.ToString());
			DrawRow(Translations.PF_RegionExtentsClose, region.extentsClose.ToString());
			DrawRow(Translations.PF_RegionExtentsLimit, region.extentsLimit.ToString());
			DrawDivider();

			District district = region.District;
			string districtRegionType = Enum.GetName(typeof(RegionType), district.RegionType);
			DrawRow(Translations.PF_District, district.ID.ToString());
			DrawRow(Translations.PF_DistrictRegionType, districtRegionType);
			DrawRow(Translations.PF_DistrictRegionCount, district.RegionCount.ToString());
			DrawRow(Translations.PF_DistrictCellCount, district.CellCount.ToString());
			DrawRow(Translations.PF_DistrictRegionsMapEdge, district.numRegionsTouchingMapEdge.ToString());
			Room room = district.Room;
			if (room == null)
			{
				return;
			}

			DrawDivider();
			DrawRow(Translations.PF_Room, room.ID.ToString());
			string roleStr = room.Role != null ? room.Role.label : Translations.PF_RoomRoleNone;
			DrawRow(Translations.PF_RoomRole, roleStr);
			string properStr = room.ProperRoom.ToStringYesNo();
			DrawRow(Translations.PF_RoomProper, properStr);
			string outdoorsStr = room.PsychologicallyOutdoors.ToStringYesNo();
			DrawRow(Translations.PF_RoomOutdoors, outdoorsStr);
			DrawRow(Translations.PF_RoomDistrictCount, room.DistrictCount.ToString());
		}

		private static void DrawRow(string label, string info)
		{
			float currentTextHeight = _numLines * LineHeight + LineHeight / 2.0F;
			if (_numLines % 2 == 1)
			{
				Rect rect = new Rect(WindowPadding, currentTextHeight, LabelColumnWidth + ColumnPadding + InfoColumnWidth,
					LineHeight);
				Widgets.DrawLightHighlight(rect);
			}

			GUI.color = Color.gray;
			Rect labelRect = new Rect(WindowPadding + ColumnPadding, currentTextHeight, LabelColumnWidth, LineHeight);
			Widgets.Label(labelRect, label);
			GUI.color = Color.white;
			Rect infoRect = new Rect(WindowPadding + ColumnPadding + LabelColumnWidth, currentTextHeight, InfoColumnWidth,
				LineHeight);
			Widgets.Label(infoRect, info);
			++_numLines;
		}

		private static void DrawHeader(string text)
		{
			const int extraTextHeight = 4;
			Text.Anchor = TextAnchor.UpperCenter;
			Text.Font = GameFont.Medium;
			float currentTextHeight = _numLines * LineHeight + LineHeight / 2.0F - extraTextHeight * 2;
			Rect rect = new Rect(WindowPadding, currentTextHeight, LabelColumnWidth + ColumnPadding + InfoColumnWidth,
				LineHeight + extraTextHeight);
			Widgets.Label(rect, text);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			++_numLines;
		}

		private static void DrawDivider()
		{
			float currentTextHeight = _numLines * LineHeight;
			GUI.color = Color.gray;
			Widgets.DrawLineHorizontal(0.0F, currentTextHeight + LineHeight, WindowWidth);
			GUI.color = Color.white;
			++_numLines;
		}
	}
}