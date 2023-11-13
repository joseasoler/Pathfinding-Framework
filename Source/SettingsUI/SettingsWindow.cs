using System;
using System.Collections.Generic;
using PathfindingFramework.Patches;
using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	/// <summary>
	/// The names of these labels must match the format used in the  PF_{}Tab translatable strings.
	/// </summary>
	public enum SettingsWindowTab
	{
		General,
		PawnMovement,
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

		private static PawnMovementTabUI _pawnMovementTabUI = new PawnMovementTabUI();

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

		private static void DoTabContents(Rect inRect)
		{
			switch (_tab)
			{
				case SettingsWindowTab.General:
					GeneralTabUI.Contents(inRect);
					break;
				case SettingsWindowTab.PawnMovement:
					_pawnMovementTabUI.Contents(inRect);
					break;
				case SettingsWindowTab.Debugging:
					DebuggingTabUI.Contents(inRect);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Draw additional buttons on the bottom button bar of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		private static void DrawBottomButtons(Rect inRect)
		{
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

			DrawBottomButtons(inRect);
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