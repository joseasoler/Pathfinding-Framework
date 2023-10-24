using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions
{
	/// <summary>
	/// Apply any required graphic changes.
	/// </summary>
	[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
	internal static class PawnGraphicSet_ResolveAllGraphics_Patch
	{
		private static void Postfix(PawnGraphicSet __instance)
		{
			__instance.pawn?.GraphicContext()?.ApplyGraphicChanges(__instance);
		}
	}
}