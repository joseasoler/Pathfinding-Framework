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
			, string labelId, int displayWithinCategory, Func<TDefType, bool> condition = null) where TDefType : Def
		{
			List<Dialog_InfoCard.Hyperlink> hyperlinks = new List<Dialog_InfoCard.Hyperlink>();

			List<Def> defsWithThisMovement = DefsWithMovementType.Get<TDefType>(movementDef, condition);
			foreach (var result in defsWithThisMovement)
			{
				hyperlinks.Add(new Dialog_InfoCard.Hyperlink(result));
			}

			if (hyperlinks.Count > 0)
			{
				entries.Add(new StatDrawEntry(StatCategoryDefOf.Basics, labelId.Translate(), "", "", displayWithinCategory,
					null, hyperlinks));
			}
		}

		public static List<StatDrawEntry> Get(MovementDef movementDef)
		{
			List<StatDrawEntry> entries = new List<StatDrawEntry>();
			AddStatDrawEntriesFromType<ThingDef>(movementDef, ref entries, "PawnsTabShort", 10,
				DefsWithMovementType.IsCreature);
			AddStatDrawEntriesFromType<HediffDef>(movementDef, ref entries, "Health", 20);
			AddStatDrawEntriesFromType<ThingDef>(movementDef, ref entries, "Apparel", 30, DefsWithMovementType.IsApparel);

			if (ModLister.BiotechInstalled)
			{
				AddStatDrawEntriesFromType<GeneDef>(movementDef, ref entries, "Genes", 40);
			}

			return entries;
		}
	}
}