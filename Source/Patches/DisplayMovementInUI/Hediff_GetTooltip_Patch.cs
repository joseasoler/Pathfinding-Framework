using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInUI
{
	/// <summary>
	/// Add granted locomotion to the hediff tooltip.
	/// </summary>
	[HarmonyPatch(typeof(Hediff), nameof(Hediff.GetTooltip))]
	public static class Hediff_GetTooltip_Patch
	{
		public static void Postfix(Hediff __instance, ref string __result)
		{
			MovementExtension extension = __instance.def.GetModExtension<MovementExtension>();
			if (extension != null)
			{
				__result +=
					$"\n\n{Translations.PF_GrantsMovementType.Colorize(ColoredText.TipSectionTitleColor)}\n  - {extension.movementDef.LabelCap}";
			}
		}
	}
}