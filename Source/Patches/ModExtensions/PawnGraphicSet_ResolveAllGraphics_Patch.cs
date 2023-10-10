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
		private static bool TryApplyLocomotionChanges(PawnGraphicSet graphicSet, Pawn pawn)
		{
			LocomotionExtension locomotionExtension = pawn?.LocomotionExtension();
			if (locomotionExtension?.graphicData == null || !pawn.pather.Moving)
			{
				// Locomotion graphic changes are only applied if the pawn is moving.
				return false;
			}

			LocomotionUrgency locomotion = CurrentUrgency_Util.Get(pawn);

			if (!locomotionExtension.locomotionUrgencies.Contains(locomotion))
			{
				return false;
			}

			graphicSet.nakedGraphic =
				locomotionExtension.graphicData.Graphic.GetCopy(graphicSet.nakedGraphic.drawSize,
					graphicSet.nakedGraphic.Shader);

			return true;
		}

		private static bool TryApplyTerrainTagChanges(PawnGraphicSet graphicSet, Pawn pawn)
		{
			TerrainTagGraphicExtension extension = pawn?.TerrainTagGraphicExtension();
			if (extension == null || !extension.Affects(pawn.Position.GetTerrain(pawn.Map)))
			{
				return false;
			}

			graphicSet.nakedGraphic =
				extension.graphicData.Graphic.GetCopy(graphicSet.nakedGraphic.drawSize,
					graphicSet.nakedGraphic.Shader);

			return true;
		}

		private static void Postfix(PawnGraphicSet __instance)
		{
			Pawn pawn = __instance.pawn;
			if (!TryApplyLocomotionChanges(__instance, pawn))
			{
				TryApplyTerrainTagChanges(__instance, pawn);
			}
		}
	}
}