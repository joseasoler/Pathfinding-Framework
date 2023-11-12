using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions
{
	/// <summary>
	/// Apply any required graphic changes.
	/// </summary>
	[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
	public static class PawnGraphicSet_ResolveAllGraphics_Patch
	{
		public static void Postfix(PawnGraphicSet __instance)
		{
			__instance.pawn?.GraphicContext()?.ApplyGraphicChanges(__instance);
		}
	}
}