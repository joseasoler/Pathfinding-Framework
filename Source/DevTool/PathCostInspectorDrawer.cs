using System;
using PathfindingFramework.Cache.Local;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PathfindingFramework.Debug
{
	/// <summary>
	/// Shows a path cost inspector drawer when a mod setting is enabled and a hotkey is kept pressed.
	/// See Verse.CellInspectorDrawer for the vanilla implementation of the map inspector.
	/// </summary>
	public class PathCostInspectorDrawer
	{
		private const float DistFromMouse = 26.0F;
		private const float LabelColumnWidth = 160.0F;
		private const float InfoColumnWidth = 100.0F;
		private const float WindowPadding = 12.0F;
		private const float ColumnPadding = 12.0F;
		private const float LineHeight = 24.0F;
		private const float WindowWidth = LabelColumnWidth + InfoColumnWidth + WindowPadding * 2 + ColumnPadding;
		private const float OutsideScreenOffset = 52.0F;

		/// <summary>
		/// True while Control+Shift+Q is held and the relevant mod setting is enabled.
		/// </summary>
		public static bool active;

		/// <summary>
		/// Current number of lines of the inspector.
		/// </summary>
		private static int numLines;

		public static void Update()
		{
			if (!Settings.Values.PathCostInspector || Current.ProgramState != ProgramState.Playing ||
			    WorldRendererUtility.WorldRenderedNow || Find.CurrentMap == null || !UI.MouseCell().InBounds(Find.CurrentMap))
			{
				active = false;
				return;
			}

			active = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
				(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.Q);

			if (active)
			{
				GenUI.RenderMouseoverBracket();
			}
		}

		public static void OnGui()
		{
			if (!active || Mouse.IsInputBlockedNow)
			{
				return;
			}

			var rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, WindowWidth,
				numLines * LineHeight + LineHeight);

			numLines = 0;
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

			Find.WindowStack.ImmediateWindow(62348, rect, WindowLayer.Super, new Action(FillWindow));
		}

		private static void FillWindow()
		{
			if (!active)
			{
				return;
			}

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;

			var map = Find.CurrentMap;
			var mapPathCostCache = MapPathCostCache.Get(map.uniqueID);
			var cell = UI.MouseCell();
			var cellIndex = map.cellIndices.CellToIndex(UI.MouseCell());
			var snowCost = SnowUtility.MovementTicksAddOn(map.snowGrid.GetCategory(cell));
			var fireCost = mapPathCostCache.FireCost(cellIndex);
			var thingsCost = mapPathCostCache.ThingsCost(cellIndex);
			var nonIgnoreRepeaterThingsCost = mapPathCostCache.NonIgnoreRepeaterThingsCost(cellIndex);
			var hasIgnoreRepeater = mapPathCostCache.HasIgnoreRepeater(cellIndex) ? "Yes" : "No";
			var hasDoor = mapPathCostCache.HasDoor(cellIndex) ? "Yes" : "No";

			DrawHeader((string)"PF_PathCostsLabel".Translate());
			DrawRow((string)"Snow".Translate(), snowCost.ToString());
			DrawRow((string)"PF_FirePathCostLabel".Translate(), fireCost.ToString());
			DrawRow((string)"PF_ThingsPathCostLabel".Translate(), thingsCost.ToString());
			DrawRow((string)"PF_NonIgnoreRepeatersPathCostLabel".Translate(), nonIgnoreRepeaterThingsCost.ToString());
			DrawRow((string)"PF_HasIgnoreRepeatersLabel".Translate(), hasIgnoreRepeater.Translate());
			DrawRow((string)"PF_HasDoorLabel".Translate(), hasDoor.Translate());

			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		/// <summary>
		/// ToDo
		/// </summary>
		/// <param name="label"></param>
		/// <param name="info"></param>
		private static void DrawRow(string label, string info)
		{
			var currentTextHeight = numLines * LineHeight + LineHeight / 2.0F;
			if (numLines % 2 == 1)
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
			++numLines;
		}

		private static void DrawHeader(string text)
		{
			const int extraTextHeight = 4;
			Text.Anchor = TextAnchor.UpperCenter;
			Text.Font = GameFont.Medium;
			var currentTextHeight = numLines * LineHeight + LineHeight / 2.0F - extraTextHeight * 2;
			var rect = new Rect(WindowPadding, currentTextHeight, LabelColumnWidth + ColumnPadding + InfoColumnWidth,
				LineHeight + extraTextHeight);
			Widgets.Label(rect, text);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			++numLines;
		}
	}
}