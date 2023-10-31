using System.Collections.Generic;
using HarmonyLib;
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
				ThingDef apparelDef = apparelList[index].def;
				MovementDef extensionMovementDef = apparelDef.MovementDef();
				if (extensionMovementDef != null && extensionMovementDef == movementDef)
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
				Gene gene = genes[index];
				if (!gene.Active)
				{
					continue;
				}

				var extensionMovementDef = gene.def.MovementDef();
				if (extensionMovementDef != null && extensionMovementDef == movementDef)
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

			GeneDef xenogeneDef = MovementFromGeneList(pawn.genes.Xenogenes, movementDef);
			if (xenogeneDef != null)
			{
				return new Pair<string, Dialog_InfoCard.Hyperlink?>("PF_GrantedByXenogene".Translate(xenogeneDef.label),
					new Dialog_InfoCard.Hyperlink(xenogeneDef));
			}

			GeneDef endogeneDef = MovementFromGeneList(pawn.genes.Endogenes, movementDef);
			if (endogeneDef != null)
			{
				return new Pair<string, Dialog_InfoCard.Hyperlink?>("PF_GrantedByEndogene".Translate(endogeneDef.label),
					new Dialog_InfoCard.Hyperlink(endogeneDef));
			}

			return null;
		}

		private static Pair<string, Dialog_InfoCard.Hyperlink?>? MovementFromHediffs(Pawn pawn, MovementDef movementDef)
		{
			List<Hediff> hediffList = pawn.health?.hediffSet?.hediffs;
			if (hediffList == null)
			{
				return null;
			}

			for (int index = 0; index < hediffList.Count; ++index)
			{
				var hediffDef = hediffList[index].def;

				var extensionMovementDef = hediffDef.MovementDef();
				if (extensionMovementDef != null && extensionMovementDef == movementDef)
				{
					return new Pair<string, Dialog_InfoCard.Hyperlink?>(hediffDef.label,
						new Dialog_InfoCard.Hyperlink(hediffDef));
				}
			}

			return null;
		}


		private static Pair<string, Dialog_InfoCard.Hyperlink?>? GetGrantedByInfo(Pawn pawn, MovementDef movementDef)
		{
			// This function must query potential granted movement types in the same order as PawnMovementUpdater.Update.
			Pair<string, Dialog_InfoCard.Hyperlink?>? apparelResult = MovementFromApparel(pawn, movementDef);
			if (apparelResult != null)
			{
				return apparelResult;
			}

			if (ModLister.BiotechInstalled)
			{
				Pair<string, Dialog_InfoCard.Hyperlink?>? geneResult = MovementFromGenes(pawn, movementDef);
				if (geneResult != null)
				{
					return geneResult;
				}
			}

			return MovementFromHediffs(pawn, movementDef);
		}

		internal static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> values, Pawn __instance)
		{
			foreach (StatDrawEntry value in values)
			{
				yield return value;
			}

			MovementDef movementDef = __instance?.MovementDef();

			if (movementDef == null)
			{
				yield break;
			}

			List<Dialog_InfoCard.Hyperlink> hyperlinks = new List<Dialog_InfoCard.Hyperlink> { new(movementDef) };

			string extraReportText = "";
			Pair<string, Dialog_InfoCard.Hyperlink?>? extraReportInfo = GetGrantedByInfo(__instance, movementDef);
			if (extraReportInfo != null)
			{
				extraReportText = extraReportInfo.Value.First;
				if (extraReportInfo.Value.Second.HasValue)
				{
					hyperlinks.Add(extraReportInfo.Value.Second.Value);
				}
			}

			string reportText = "PF_MovementReportText".Translate();
			if (!extraReportText.NullOrEmpty())
			{
				reportText += "PF_GrantedBy".Translate(extraReportText);
			}

			yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "PF_Movement".Translate(),
				movementDef.LabelCap, reportText, 2501, null, hyperlinks);
		}
	}
}