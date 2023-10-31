using PathfindingFramework.Patches;
using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Implementation of the mod settings window.
	/// </summary>
	public static class SettingsWindow
	{
		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public static string SettingsCategory()
		{
			return PathfindingFrameworkMod.Name;
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public static void DoWindowContents(Rect inRect)
		{
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_IgnoreFireLabel".Translate(), ref Settings.Values.IgnoreFire,
				"PF_IgnoreFireHover".Translate());

			listing.Gap();

			listing.CheckboxLabeled("PF_InspectorLabel".Translate(), ref Settings.Values.Inspectors,
				"PF_InspectorHover".Translate());
			listing.CheckboxLabeled("PF_LogPathNotFoundLabel".Translate(), ref Settings.Values.LogPathNotFound,
				"PF_LogPathNotFoundHover".Translate());
			listing.CheckboxLabeled("PF_DebugLogLabel".Translate(), ref Settings.Values.DebugLog,
				"PF_DebugLogHover".Translate());

			listing.Gap();
			var buttonsRect = listing.GetRect(30.0F);
			var buttonWidth = buttonsRect.width / 5.0F;

			var resetRect = new Rect(buttonsRect.width - buttonWidth, buttonsRect.y, buttonWidth, buttonsRect.height);
			if (Widgets.ButtonText(resetRect, "PF_ResetSettingsLabel".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetRect, "PF_ResetSettingsHover".Translate());

			listing.End();
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