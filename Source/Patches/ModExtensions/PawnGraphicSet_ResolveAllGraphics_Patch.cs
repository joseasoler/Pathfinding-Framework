using HarmonyLib;
using PathfindingFramework.Patches.ModExtensions.Locomotion;
using Verse;
using Verse.AI;

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
			Pawn pawn = __instance.pawn;
			LocomotionExtension locomotionExtension = pawn?.LocomotionExtension();
			if (locomotionExtension?.graphicData == null || !pawn.pather.Moving)
			{
				// Locomotion graphic changes are only applied if the pawn is moving.
				return;
			}

			LocomotionUrgency locomotion = CurrentUrgency_Util.Get(pawn);

			if (!locomotionExtension.locomotionUrgencies.Contains(locomotion))
			{
				return;
			}

			__instance.nakedGraphic = locomotionExtension.graphicData.Graphic;
		}
	}
}