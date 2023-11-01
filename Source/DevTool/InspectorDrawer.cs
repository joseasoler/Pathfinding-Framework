using System;
using System.Collections.Generic;
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

			var rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, WindowWidth,
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

			string titleId;
			if (_pathCostInspectorActive)
			{
				titleId = "PF_PathCostsLabel";
			}
			else if (_movementContextInspectorActive)
			{
				titleId = "PF_MovementCostsLabel";
			}
			else
			{
				titleId = "PF_RegionLabel";
			}

			DrawHeader(titleId.Translate());
			// Draw shared initial properties.
			DrawRow("PF_CellLabel".Translate(), $"{cell.x}, {cell.z} ({cellIndex})");

			DrawDivider();
			DrawRow("Terrain_Label".Translate(), terrainDef.LabelCap);

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
				: "Impassable".Translate();
			DrawRow("PF_BasePathCost".Translate(), basePathCostLabel);
			for (int dataIndex = 0; dataIndex < movementTypeLabels.Count; ++dataIndex)
			{
				DrawRow("PF_TerrainCost".Translate(movementTypeLabels[dataIndex]), PathCostLabel(terrainPathCosts[dataIndex]));
			}

			DrawDivider();
			DrawRow("Snow_Label".Translate(), mapPathCost.snow.ToString());
			DrawRow("PF_FirePathCostLabel".Translate(), mapPathCost.fire.ToString());
			DrawRow("PF_ThingsPathCostLabel".Translate(), PathCostLabel(mapPathCost.things));
			DrawRow("PF_NonIgnoreRepeatersPathCostLabel".Translate(), PathCostLabel(mapPathCost.nonIgnoreRepeaterThings));
			var hasIgnoreRepeater = mapPathCost.hasIgnoreRepeater ? "Yes" : "No";
			DrawRow("PF_HasIgnoreRepeatersLabel".Translate(), hasIgnoreRepeater.Translate());
			var hasDoor = mapPathCost.hasDoor ? "Yes" : "No";
			DrawRow("PF_HasDoorLabel".Translate(), hasDoor.Translate());
			var hasFence = mapPathCost.hasFence ? "Yes" : "No";
			DrawRow("PF_HasFenceLabel".Translate(), hasFence.Translate());
		}

		private static void FillMovementContextInspectorWindow(Map map, IntVec3 cell)
		{
			DrawDivider();
			short vanillaNormalCost = (short)map.pathing.Normal.pathGrid.PerceivedPathCostAt(cell);
			short vanillaFenceCost = (short)map.pathing.FenceBlocked.pathGrid.PerceivedPathCostAt(cell);
			string vanillaLabel = "PF_VanillaLabel".Translate();
			DrawRow(vanillaLabel, PathCostLabel(vanillaNormalCost));
			DrawRow("PF_NoFencesMovementLabel".Translate(vanillaLabel), PathCostLabel(vanillaFenceCost));

			DrawDivider();
			List<MovementContext> activeContexts = map.MovementContextData().ActiveContexts();
			activeContexts.Sort((contextLhs, contextRhs) =>
				contextLhs.MovementDef.priority.CompareTo(contextRhs.MovementDef.priority));

			foreach (MovementContext context in activeContexts)
			{
				short cost = (short)context.PathingContext.pathGrid.PerceivedPathCostAt(cell);
				string movementLabelCap = context.MovementDef.LabelCap;
				List<string> extraLabels = new List<string>();
				if (context.ShouldAvoidFences)
				{
					extraLabels.Add("PF_NoFencesMovementLabel".Translate());
				}

				if (context.CanIgnoreFire)
				{
					extraLabels.Add("PF_IgnoreFireMovementLabel".Translate());
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
				DrawRow("PF_RegionType".Translate(), "PF_RegionNone".Translate());
				return;
			}

			string regionType = Enum.GetName(typeof(RegionType), region.type);

			DrawRow("PF_RegionId".Translate(), region.id.ToString());
			DrawRow("PF_RegionType".Translate(), regionType);
			DrawRow("PF_RegionLinks".Translate(), region.links.Count.ToString());
			DrawRow("PF_RegionExtentsClose".Translate(), region.extentsClose.ToString());
			DrawRow("PF_RegionExtentsLimit".Translate(), region.extentsLimit.ToString());
			DrawDivider();

			District district = region.District;
			string districtRegionType = Enum.GetName(typeof(RegionType), district.RegionType);
			DrawRow("PF_District".Translate(), district.ID.ToString());
			DrawRow("PF_DistrictRegionType".Translate(), districtRegionType);
			DrawRow("PF_DistrictRegionCount".Translate(), district.RegionCount.ToString());
			DrawRow("PF_DistrictCellCount".Translate(), district.CellCount.ToString());
			DrawRow("PF_DistrictRegionsMapEdge".Translate(), district.numRegionsTouchingMapEdge.ToString());
		}

		private static void DrawRow(string label, string info)
		{
			var currentTextHeight = _numLines * LineHeight + LineHeight / 2.0F;
			if (_numLines % 2 == 1)
			{
				var rect = new Rect(WindowPadding, currentTextHeight, LabelColumnWidth + ColumnPadding + InfoColumnWidth,
					LineHeight);
				Widgets.DrawLightHighlight(rect);
			}

			GUI.color = Color.gray;
			var labelRect = new Rect(WindowPadding + ColumnPadding, currentTextHeight, LabelColumnWidth, LineHeight);
			Widgets.Label(labelRect, label);
			GUI.color = Color.white;
			var infoRect = new Rect(WindowPadding + ColumnPadding + LabelColumnWidth, currentTextHeight, InfoColumnWidth,
				LineHeight);
			Widgets.Label(infoRect, info);
			++_numLines;
		}

		private static void DrawHeader(string text)
		{
			const int extraTextHeight = 4;
			Text.Anchor = TextAnchor.UpperCenter;
			Text.Font = GameFont.Medium;
			var currentTextHeight = _numLines * LineHeight + LineHeight / 2.0F - extraTextHeight * 2;
			var rect = new Rect(WindowPadding, currentTextHeight, LabelColumnWidth + ColumnPadding + InfoColumnWidth,
				LineHeight + extraTextHeight);
			Widgets.Label(rect, text);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			++_numLines;
		}

		private static void DrawDivider()
		{
			var currentTextHeight = _numLines * LineHeight;
			GUI.color = Color.gray;
			Widgets.DrawLineHorizontal(0.0F, currentTextHeight + LineHeight, WindowWidth);
			GUI.color = Color.white;
			++_numLines;
		}
	}
}