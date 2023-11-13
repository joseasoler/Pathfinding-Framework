using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	public static class GeneralTabUI
	{
		public static void Contents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_IgnoreFireLabel".Translate(), ref Settings.Values.IgnoreFire,
				"PF_IgnoreFireHover".Translate());

			listing.End();
		}
	}
}