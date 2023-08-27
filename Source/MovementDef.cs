using System;
using System.Collections.Generic;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// A movement type is defined by a set of custom pathing rules that pawns must follow.
	/// </summary>
	public class MovementDef : Verse.Def
	{
		/// <summary>
		/// Used to choose when two or more movement types are available.
		/// </summary>
		public int priority;

		/// <summary>
		/// Maps terrain tags to their path costs in this movement type.
		/// Setting a value lower than 10000 for an impassable terrain will let it be passable for this movement type.
		/// If a terrain has more than one matching tag, the largest tag value will be used.
		/// </summary>
		public TerrainTagPathCosts tagCosts;

		/// <summary>
		/// By default, passable terrains not affected by other movement type changes will use the costs defined in their
		/// defs. If this field is set to a value equal or larger than zero, this will be the default path cost instead.
		/// defaultPathCost cannot make impassable terrain passable.
		/// </summary>
		public PathCost defaultCost;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			if (defaultCost == PathCost.Invalid)
			{
				yield return Report.ConfigError(this, $"defaultCost must be a numeric value or a valid PathingCost.");
			}

			if (tagCosts != null)
			{
				foreach (var tagCost in tagCosts.data)
				{
					if (tagCost.Value == PathCost.Invalid)
					{
						yield return Report.ConfigError(this,
							$"tagCost {tagCost.Key} must be a numeric value or a valid PathingCost. But it was {tagCost.Value}");
					}
				}
			}
		}

		private List<Def> MovementDefsFromType<TDefType>(Func<TDefType, bool> condition = null)
			where TDefType : Def
		{
			var defs = DefDatabase<TDefType>.AllDefsListForReading;
			var resultList = new List<Def>();
			for (int index = 0; index < defs.Count; ++index)
			{
				var currentDef = defs[index];
				var extension = currentDef.GetModExtension<MovementExtension>();
				if (!currentDef.generated && extension != null && extension.movementDef == this &&
				    (condition == null || condition(currentDef)))
				{
					resultList.Add(currentDef);
				}
			}

			return resultList;
		}

		private void AddDefHyperlinksFromType<TDefType>(ref List<DefHyperlink> hyperlinks,
			Func<TDefType, bool> condition = null) where TDefType : Def
		{
			var resultList = MovementDefsFromType<TDefType>(condition);
			foreach (var result in resultList)
			{
				hyperlinks.Add(new DefHyperlink(result));
			}
		}

		private static bool IsApparel(ThingDef thingDef)
		{
			return thingDef.IsApparel;
		}

		private static bool IsCreature(ThingDef thingDef)
		{
			return thingDef.race != null;
		}

		public override void ResolveReferences()
		{
			if (this.descriptionHyperlinks == null)
			{
				this.descriptionHyperlinks = new List<DefHyperlink>();
			}

			AddDefHyperlinksFromType<ThingDef>(ref this.descriptionHyperlinks, IsCreature);
			AddDefHyperlinksFromType<HediffDef>(ref this.descriptionHyperlinks);
			AddDefHyperlinksFromType<ThingDef>(ref this.descriptionHyperlinks, IsApparel);

			if (ModLister.BiotechInstalled)
			{
				AddDefHyperlinksFromType<GeneDef>(ref this.descriptionHyperlinks);
			}
		}

		private List<Dialog_InfoCard.Hyperlink> HyperlinksFromType<TDefType>(Func<TDefType, bool> condition = null)
			where TDefType : Def
		{
			var resultList = MovementDefsFromType<TDefType>(condition);
			if (resultList.Count == 0)
			{
				return null;
			}

			var hyperlinks = new List<Dialog_InfoCard.Hyperlink>();

			foreach (var result in resultList)
			{
				hyperlinks.Add(new Dialog_InfoCard.Hyperlink(result));
			}

			return hyperlinks;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}

			var raceHyperlinks = HyperlinksFromType<ThingDef>(IsCreature);
			if (raceHyperlinks != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "PawnsTabShort".Translate(), "", "", 10, null,
					raceHyperlinks);
			}

			var hediffHyperlinks = HyperlinksFromType<HediffDef>();
			if (hediffHyperlinks != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Health".Translate(), "", "", 20, null,
					hediffHyperlinks);
			}

			var apparelHyperlinks = HyperlinksFromType<ThingDef>(IsApparel);
			if (apparelHyperlinks != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Apparel".Translate(), "", "", 30, null,
					apparelHyperlinks);
			}

			if (ModLister.BiotechInstalled)
			{
				var geneHyperlinks = HyperlinksFromType<GeneDef>();
				if (geneHyperlinks != null)
				{
					yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Genes".Translate(), "", "", 40, null,
						geneHyperlinks);
				}
			}
		}
	}
}