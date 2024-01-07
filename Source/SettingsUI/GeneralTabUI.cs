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

			listing.CheckboxLabeled(Translations.PF_IgnoreFireLabel, ref Settings.Values.IgnoreFire,
				Translations.PF_IgnoreFireHover);

			listing.CheckboxLabeled(Translations.PF_WildAnimalRelocationLabel, ref Settings.Values.WildAnimalRelocating,
				Translations.PF_WildAnimalRelocationHover);

			listing.End();
		}
	}
}