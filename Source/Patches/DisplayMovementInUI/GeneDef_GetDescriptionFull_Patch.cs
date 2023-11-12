using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInUI
{
	/// <summary>
	/// Add granted locomotion to the description of genes.
	/// </summary>
	[HarmonyPatch(typeof(GeneDef), "GetDescriptionFull")]
	public static class GeneDef_GetDescriptionFull_Patch
	{
		public static void Postfix(GeneDef __instance, ref string __result)
		{
			MovementExtension extension = __instance.GetModExtension<MovementExtension>();
			if (extension != null)
			{
				__result +=
					$"\n\n{"PF_GrantsMovementType".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}\n  - {extension.movementDef.LabelCap}";
			}
		}
	}
}