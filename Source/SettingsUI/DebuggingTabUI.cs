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

			listing.CheckboxLabeled("PF_InspectorLabel".Translate(), ref Settings.Values.Inspectors,
				"PF_InspectorHover".Translate());
			listing.CheckboxLabeled("PF_LogPathNotFoundLabel".Translate(), ref Settings.Values.LogPathNotFound,
				"PF_LogPathNotFoundHover".Translate());
			listing.CheckboxLabeled("PF_DebugLogLabel".Translate(), ref Settings.Values.DebugLog,
				"PF_DebugLogHover".Translate());

			listing.End();
		}
	}
}