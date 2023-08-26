using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInUI
{
	[HarmonyPatch(typeof(GeneDef), "GetDescriptionFull")]
	public class GeneDef_GetDescriptionFull_Patch
	{
		internal static void Postfix(GeneDef __instance, ref string __result)
		{
			var extension = __instance.GetModExtension<MovementExtension>();
			if (extension != null)
			{
				__result +=
					$"\n\n{"PF_GrantsLocomotion".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}\n  - {extension.movementDef.LabelCap}";
			}
		}
	}
}