﻿using System;
using System.Collections.Generic;
using PathfindingFramework.Patches;
using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// The names of these labels must match the format used in the  PF_{}Tab translatable strings.
	/// </summary>
	public enum SettingsWindowTab
	{
		General,
		Debugging,
	}

	/// <summary>
	/// Implementation of the mod settings window.
	/// </summary>
	public static class SettingsWindow
	{
		/// <summary>
		/// Space taken by the title of the mod in the mod settings window.
		/// </summary>
		private const float TitleHeight = GenUI.ListSpacing;

		/// <summary>
		/// Height of the tabs part of the settings window.
		/// </summary>
		private const float TabsHeight = GenUI.ListSpacing;

		/// <summary>
		/// Current tab shown in the UI.
		/// </summary>
		private static SettingsWindowTab _tab = SettingsWindowTab.General;

		/// <summary>
		/// Used to trigger a write settings call when tabs change.
		/// </summary>
		private static Action _writeSettingsAction;

		/// <summary>
		/// Initialize the settings window. Should be called only once before it is shown.
		/// </summary>
		/// <param name="writeSettingsAction">Action to use to trigger a write settings call.</param>
		public static void Initialize(Action writeSettingsAction)
		{
			_writeSettingsAction = writeSettingsAction;
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public static string SettingsCategory()
		{
			return PathfindingFrameworkMod.Name;
		}

		private static List<TabRecord> Tabs()
		{
			Type enumType = typeof(SettingsWindowTab);
			List<TabRecord> tabs = new List<TabRecord>();
			foreach (SettingsWindowTab tab in Enum.GetValues(enumType))
			{
				string tabStr = Enum.GetName(enumType, tab);
				tabs.Add(new TabRecord($"PF_{tabStr}Tab".Translate(), () =>
				{
					_tab = tab;
					_writeSettingsAction();
				}, _tab == tab));
			}

			return tabs;
		}

		private static void DoGeneralTabContents(Listing_Standard listing)
		{
			listing.CheckboxLabeled("PF_IgnoreFireLabel".Translate(), ref Settings.Values.IgnoreFire,
				"PF_IgnoreFireHover".Translate());
		}

		private static void DoDebuggingTabContents(Listing_Standard listing)
		{
			listing.CheckboxLabeled("PF_InspectorLabel".Translate(), ref Settings.Values.Inspectors,
				"PF_InspectorHover".Translate());
			listing.CheckboxLabeled("PF_LogPathNotFoundLabel".Translate(), ref Settings.Values.LogPathNotFound,
				"PF_LogPathNotFoundHover".Translate());
			listing.CheckboxLabeled("PF_DebugLogLabel".Translate(), ref Settings.Values.DebugLog,
				"PF_DebugLogHover".Translate());
		}

		private static void DoTabContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			switch (_tab)
			{
				case SettingsWindowTab.General:
					DoGeneralTabContents(listing);
					break;
				case SettingsWindowTab.Debugging:
					DoDebuggingTabContents(listing);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			listing.End();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public static void DoWindowContents(Rect inRect)
		{
			Rect settingsArea = inRect.BottomPartPixels(inRect.height - TitleHeight);
			Rect tabArea = settingsArea.TopPartPixels(TabsHeight);

			Widgets.DrawMenuSection(settingsArea);
			TabDrawer.DrawTabs(tabArea, Tabs());

			DoTabContents(settingsArea.ContractedBy(15.0f));

			// Draw the reset button at the right side of the area that Dialog_ModSettings reserves for the close button.

			// The reset button is placed at the bottom right corner.
			float resetX = inRect.width - Window.CloseButSize.x;
			// Dialog_ModSettings leaves a margin of Window.CloseButSize.y at the bottom for the close button.
			// Then, there are three pixels between the top border of the close button and the rest of this window.
			float resetY = inRect.height + Window.CloseButSize.y + 3;
			Rect resetButtonArea = new Rect(resetX, resetY, Window.CloseButSize.x, Window.CloseButSize.y);

			if (Widgets.ButtonText(resetButtonArea, "PF_ResetSettingsLabel".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonArea, "PF_ResetSettingsHover".Translate());
		}

		public static void OnWriteSettings()
		{
			if (Find.Maps == null)
			{
				return;
			}

			foreach (Map map in Find.Maps)
			{
				map.MovementContextData().UpdateAllCells(ignoreFireOnly: true);
			}
		}
	}
}