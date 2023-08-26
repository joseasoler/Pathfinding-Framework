using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	public static class SettingsWindow
	{
		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public static string SettingsCategory()
		{
			return Mod.Name;
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public static void DoWindowContents(Rect inRect)
		{
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_PathCostInspectorLabel".Translate(), ref Settings.Values.PathCostInspector,
				"PF_PathCostInspectorHover".Translate());
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
	}
}