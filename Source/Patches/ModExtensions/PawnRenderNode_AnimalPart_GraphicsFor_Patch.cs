using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions
{
	/// <summary>
	/// Apply any required graphic changes.
	/// </summary>
	[HarmonyPatch(typeof(PawnRenderNode_AnimalPart), nameof(PawnRenderNode_AnimalPart.GraphicFor))]
	public static class PawnRenderNode_AnimalPart_GraphicsFor_Patch
	{
		public static void Postfix(Pawn pawn, ref Graphic __result)
		{
			Graphic graphic = pawn?.GraphicContext()?.TryGetGraphicChange(__result);
			if (graphic != null)
			{
				__result = graphic;
			}
		}
	}
}