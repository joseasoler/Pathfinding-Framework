using System.Collections.Generic;
using PathfindingFramework.MapPathCosts;
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
	public class PathCostInspectorDrawer
	{
		private const float DistFromMouse = 26.0F;
		private const float LabelColumnWidth = 160.0F;
		private const float InfoColumnWidth = 140.0F;
		private const float WindowPadding = 12.0F;
		private const float ColumnPadding = 12.0F;
		private const float LineHeight = 24.0F;
		private const float WindowWidth = LabelColumnWidth + InfoColumnWidth + WindowPadding * 2 + ColumnPadding;
		private const float OutsideScreenOffset = 52.0F;

		/// <summary>
		/// True while Control+Shift+Q is held and the relevant mod setting is enabled.
		/// </summary>
		private static bool _active;

		/// <summary>
		/// Current number of lines of the inspector.
		/// </summary>
		private static int _numLines;

		public static void Update()
		{
			if (!Settings.Values.PathCostInspector || Current.ProgramState != ProgramState.Playing ||
			    WorldRendererUtility.WorldRenderedNow || Find.CurrentMap == null || !UI.MouseCell().InBounds(Find.CurrentMap))
			{
				_active = false;
				return;
			}

			_active = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
			          (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.Q);

			if (_active)
			{
				GenUI.RenderMouseoverBracket();
			}
		}

		public static void OnGui()
		{
			if (!_active || Mouse.IsInputBlockedNow)
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
			string movementPathCostLabel;
			switch ((PathCostValues) pathCost)
			{
				case PathCostValues.Avoid:
					movementPathCostLabel = "Avoid";
					break;
				case PathCostValues.Impassable:
					movementPathCostLabel = "Impassable";
					break;
				default:
					movementPathCostLabel = pathCost.ToString();
					break;
			}

			return movementPathCostLabel;
		}

		private static void FillWindow()
		{
			if (!_active)
			{
				return;
			}

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;

			Map map = Find.CurrentMap;
			IntVec3 cell = UI.MouseCell();
			int cellIndex = map.cellIndices.CellToIndex(UI.MouseCell());
			MapPathCostGrid mapPathCostGrid = map.MapPathCostGrid();
			MapPathCost mapPathCost = mapPathCostGrid.Get(cellIndex);

			// Terrain path cost calculation.
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(cellIndex);
			List<string> movementTypeLabels = new List<string>();
			List<short> terrainPathCosts = new List<short>();
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			for (int movementIndex = 0; movementIndex < movementDefs.Count; ++movementIndex)
			{
				MovementDef movementDef = movementDefs[movementIndex];
				string label = movementDef.LabelCap;
				movementTypeLabels.Add(label);
				short terrainPathCost = movementDef.PathCosts[terrainDef.index];
				terrainPathCosts.Add(terrainPathCost);
			}

			DrawHeader("PF_PathCostsLabel".Translate());
			DrawRow("PF_CellLabel".Translate(), $"{cell.x}, {cell.z} ({cellIndex})");

			DrawDivider();
			DrawRow("Terrain_Label".Translate(), terrainDef.LabelCap);
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

			/* ToDo move to a path grid inspector.
			DrawDivider();
			var vanillaNormalCost = map.pathing.Normal.pathGrid.PerceivedPathCostAt(cell);
			var vanillaFenceCost = map.pathing.FenceBlocked.pathGrid.PerceivedPathCostAt(cell);
			DrawRow("PF_VanillaNormalCostLabel".Translate(), vanillaNormalCost.ToString());
			DrawRow("PF_VanillaFencesCostLabel".Translate(), vanillaFenceCost.ToString());
			*/

			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
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