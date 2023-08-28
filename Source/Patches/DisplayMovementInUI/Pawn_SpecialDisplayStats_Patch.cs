using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.Cache.Global;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInUI
{
	/// <summary>
	/// Display information about the movement type in the pawn information window.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpecialDisplayStats))]
	internal static class Pawn_SpecialDisplayStats_Patch
	{
		private static Pair<string, Dialog_InfoCard.Hyperlink?>? MovementFromApparel(Pawn pawn, MovementDef movementDef)
		{
			var apparelList = pawn.apparel?.WornApparel;
			if (apparelList == null)
			{
				return null;
			}

			for (int index = 0; index < apparelList.Count; ++index)
			{
				var apparelDef = apparelList[index].def;
				var extension = MovementExtensionCache.GetExtension(apparelDef);
				if (extension != null && extension.movementDef == movementDef)
				{
					return new Pair<string, Dialog_InfoCard.Hyperlink?>(apparelDef.label,
						new Dialog_InfoCard.Hyperlink(apparelDef));
				}
			}

			return null;
		}

		private static GeneDef MovementFromGeneList(List<Gene> genes,
			MovementDef movementDef)
		{
			for (int index = 0; index < genes.Count; ++index)
			{
				var gene = genes[index];
				if (!gene.Active)
				{
					continue;
				}

				var extension = MovementExtensionCache.GetExtension(gene.def);
				if (extension != null && extension.movementDef == movementDef)
				{
					return gene.def;
				}
			}

			return null;
		}

		private static Pair<string, Dialog_InfoCard.Hyperlink?>? MovementFromGenes(Pawn pawn, MovementDef movementDef)
		{
			if (pawn.genes == null)
			{
				return null;
			}

			var xenogeneDef = MovementFromGeneList(pawn.genes.Xenogenes, movementDef);
			if (xenogeneDef != null)
			{
				return new Pair<string, Dialog_InfoCard.Hyperlink?>("PF_GrantedByXenogene".Translate(xenogeneDef.label),
					new Dialog_InfoCard.Hyperlink(xenogeneDef));
			}

			var endogeneDef = MovementFromGeneList(pawn.genes.Endogenes, movementDef);
			if (endogeneDef != null)
			{
				return new Pair<string, Dialog_InfoCard.Hyperlink?>("PF_GrantedByEndogene".Translate(endogeneDef.label),
					new Dialog_InfoCard.Hyperlink(endogeneDef));
			}

			return null;
		}

		private static Pair<string, Dialog_InfoCard.Hyperlink?>? MovementFromHediffs(Pawn pawn, MovementDef movementDef)
		{
			var hediffList = pawn.health?.hediffSet?.hediffs;
			if (hediffList == null)
			{
				return null;
			}

			for (var index = 0; index < hediffList.Count; ++index)
			{
				var hediffDef = hediffList[index].def;

				var extension = MovementExtensionCache.GetExtension(hediffDef);
				if (extension != null && extension.movementDef == movementDef)
				{
					return new Pair<string, Dialog_InfoCard.Hyperlink?>(hediffDef.label,
						new Dialog_InfoCard.Hyperlink(hediffDef));
				}
			}

			return null;
		}

		private static Pair<string, Dialog_InfoCard.Hyperlink?>? GetExtraReportInfo(Pawn pawn, MovementDef movementDef)
		{
			var apparelResult = MovementFromApparel(pawn, movementDef);
			if (apparelResult != null)
			{
				return apparelResult;
			}

			if (ModLister.BiotechInstalled)
			{
				var geneResult = MovementFromGenes(pawn, movementDef);
				if (geneResult != null)
				{
					return geneResult;
				}
			}

			return MovementFromHediffs(pawn, movementDef);
		}

		internal static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> values, Pawn __instance)
		{
			foreach (var value in values)
			{
				yield return value;
			}

			var movementIndex = PawnMovementCache.Get(__instance);
			var movementDef = DefDatabase<MovementDef>.AllDefsListForReading[movementIndex];

			var hyperlinks = new List<Dialog_InfoCard.Hyperlink> { new Dialog_InfoCard.Hyperlink(movementDef) };

			var extraReportText = "";
			var extraReportInfo = GetExtraReportInfo(__instance, movementDef);
			if (extraReportInfo != null)
			{
				extraReportText = extraReportInfo.Value.First;
				if (extraReportInfo.Value.Second.HasValue)
				{
					hyperlinks.Add(extraReportInfo.Value.Second.Value);
				}
			}

			string reportText = "PF_LocomotionReportText".Translate();
			if (!extraReportText.NullOrEmpty())
			{
				reportText += "PF_GrantedBy".Translate(extraReportText);
			}

			yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "PF_Locomotion".Translate(),
				movementDef.LabelCap, reportText, 2501, null, hyperlinks);
		}
	}
}