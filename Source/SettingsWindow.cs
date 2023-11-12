using System;
using System.Collections.Generic;
using System.Text;
using PathfindingFramework.Patches;
using RimWorld;
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
		PawnMovement,
		Debugging,
	}

	/// <summary>
	/// Sorts entries of the pawn movement table. Assumes that all entries are non-null and have a race.
	/// </summary>
	public class ComparePawnMovementEntries : IComparer<Pair<ThingDef, Texture>>
	{
		public int Compare(Pair<ThingDef, Texture> lhs, Pair<ThingDef, Texture> rhs)
		{
			bool humanlikeLhs = lhs.First.race.Humanlike;
			bool humanlikeRhs = rhs.First.race.Humanlike;

			int humanlikeCompare = humanlikeLhs.CompareTo(humanlikeRhs);
			if (humanlikeCompare != 0)
			{
				return -humanlikeCompare;
			}

			bool animalLhs = lhs.First.race.Animal;
			bool animalRhs = rhs.First.race.Animal;

			int animalCompare = animalLhs.CompareTo(animalRhs);
			if (animalCompare != 0)
			{
				return -animalCompare;
			}

			return string.Compare(lhs.First.label, rhs.First.label, StringComparison.OrdinalIgnoreCase);
		}
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
		/// Current scroll position of the PawnMovement tab.
		/// </summary>
		// private static int _firstPawnMovementIndex = 0;

		/// <summary>
		/// List of entries of the pawn movement window.
		/// </summary>
		private static List<Pair<ThingDef, Texture>> _pawnMovementEntries;

		private static Texture GetTextureFromPawnKindDef(PawnKindDef pawnKindDef)
		{
			if (pawnKindDef.lifeStages == null)
			{
				return null;
			}

			int lastLifeStageIndex = pawnKindDef.lifeStages.Count - 1;
			if (lastLifeStageIndex < 0)
			{
				return null;
			}

			PawnKindLifeStage lastLifeStage = pawnKindDef.lifeStages[lastLifeStageIndex];
			return lastLifeStage.bodyGraphicData?.Graphic?.MatSingle.mainTexture;
		}

		private static bool IsValidPawnKindDef(PawnKindDef pawnKindDef, HashSet<ThingDef> ignoreDuplicates)
		{
			// Avoid showing Vehicles Framework pawns.
			const string vehiclesFrameworkPawnKindDef = "VehicleDef";

			return pawnKindDef.race?.race != null &&
				pawnKindDef.race.GetType().Name != vehiclesFrameworkPawnKindDef &&
				!ignoreDuplicates.Contains(pawnKindDef.race);
		}

		private static List<Pair<ThingDef, Texture>> GetPawnMovementEntries()
		{
			if (_pawnMovementEntries != null)
			{
				return _pawnMovementEntries;
			}

			_pawnMovementEntries = new List<Pair<ThingDef, Texture>>();
			HashSet<ThingDef> ignoreDuplicates = new HashSet<ThingDef>();

			List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
			for (int pawnIndex = 0; pawnIndex < pawnKindDefs.Count; ++pawnIndex)
			{
				PawnKindDef pawnKindDef = pawnKindDefs[pawnIndex];
				if (!IsValidPawnKindDef(pawnKindDef, ignoreDuplicates))
				{
					continue;
				}

				ThingDef raceThingDef = pawnKindDef.race;
				ignoreDuplicates.Add(raceThingDef);
				_pawnMovementEntries.Add(new Pair<ThingDef, Texture>(raceThingDef, GetTextureFromPawnKindDef(pawnKindDef)));
			}

			_pawnMovementEntries.Sort(new ComparePawnMovementEntries());
			return _pawnMovementEntries;
		}

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

		private static void DoGeneralTabContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_IgnoreFireLabel".Translate(), ref Settings.Values.IgnoreFire,
				"PF_IgnoreFireHover".Translate());

			listing.End();
		}

		private static void DoPawnMovementTabContents(Rect inRect)
		{
			// const float animalEntryHeight = 64F;

			Listing_Standard listing = new Listing_Standard();

			// ToDo leave space for the vertical scroll bar.
			listing.Begin(inRect);

			// GUI.VerticalScrollbar(new Rect(screenRect.xMax + (float) verticalScrollbar.margin.left, screenRect.y, verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y, Mathf.Min(screenRect.height, viewRect.height), 0.0f, viewRect.height, verticalScrollbar);

			listing.End();
		}

		private static void DoDebuggingTabContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_InspectorLabel".Translate(), ref Settings.Values.Inspectors,
				"PF_InspectorHover".Translate());
			listing.CheckboxLabeled("PF_LogPathNotFoundLabel".Translate(), ref Settings.Values.LogPathNotFound,
				"PF_LogPathNotFoundHover".Translate());
			listing.CheckboxLabeled("PF_DebugLogLabel".Translate(), ref Settings.Values.DebugLog,
				"PF_DebugLogHover".Translate());

			listing.End();
		}

		private static void DoTabContents(Rect inRect)
		{
			switch (_tab)
			{
				case SettingsWindowTab.General:
					DoGeneralTabContents(inRect);
					break;
				case SettingsWindowTab.PawnMovement:
					DoPawnMovementTabContents(inRect);
					break;
				case SettingsWindowTab.Debugging:
					DoDebuggingTabContents(inRect);
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