using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInUI
{
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.GetTooltip))]
	internal static class Hediff_GetTooltip_Patch
	{
		internal static void Postfix(Hediff __instance, ref string __result)
		{
			var extension = __instance.def.GetModExtension<MovementExtension>();
			if (extension != null)
			{
				__result +=
					$"\n\n{"PF_GrantsLocomotion".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}\n  - {extension.movementDef.LabelCap}";
			}
		}
	}
}