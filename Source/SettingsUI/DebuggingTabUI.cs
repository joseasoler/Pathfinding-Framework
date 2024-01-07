using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	public static class DebuggingTabUI
	{
		public static void Contents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled(Translations.PF_InspectorLabel, ref Settings.Values.Inspectors,
				Translations.PF_InspectorHover);
			listing.CheckboxLabeled(Translations.PF_LogPathNotFoundLabel, ref Settings.Values.LogPathNotFound,
				Translations.PF_LogPathNotFoundHover);
			listing.CheckboxLabeled(Translations.PF_DebugLogLabel, ref Settings.Values.DebugLog,
				Translations.PF_DebugLogHover);

			listing.End();
		}
	}
}