using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Draw stat entries of a movement def.
	/// </summary>
	public static class StatDrawEntries
	{
		private static void AddStatDrawEntriesFromType<TDefType>(MovementDef movementDef, ref List<StatDrawEntry> entries
			, string labelStr, int displayWithinCategory, Func<TDefType, bool> condition = null) where TDefType : Def
		{
			List<Dialog_InfoCard.Hyperlink> hyperlinks = new List<Dialog_InfoCard.Hyperlink>();

			List<Def> defsWithThisMovement = DefsWithMovementType.Get(movementDef, condition);
			foreach (Def result in defsWithThisMovement)
			{
				hyperlinks.Add(new Dialog_InfoCard.Hyperlink(result));
			}

			if (hyperlinks.Count > 0)
			{
				entries.Add(new StatDrawEntry(StatCategoryDefOf.Basics, labelStr, "", "", displayWithinCategory,
					null, hyperlinks));
			}
		}

		public static List<StatDrawEntry> Get(MovementDef movementDef)
		{
			List<StatDrawEntry> entries = new List<StatDrawEntry>();
			AddStatDrawEntriesFromType<ThingDef>(movementDef, ref entries, Translations.PawnsTabShort, 10,
				DefsWithMovementType.IsCreature);
			AddStatDrawEntriesFromType<HediffDef>(movementDef, ref entries, Translations.Health, 20);
			AddStatDrawEntriesFromType<ThingDef>(movementDef, ref entries, Translations.Apparel, 30,
				DefsWithMovementType.IsApparel);

			if (ModLister.BiotechInstalled)
			{
				AddStatDrawEntriesFromType<GeneDef>(movementDef, ref entries, Translations.Genes, 40);
			}

			return entries;
		}
	}
}