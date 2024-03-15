using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.PawnGraphic;
using UnityEngine;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions.MovementMultiplier
{
	/// <summary>
	/// Apply move speed stat offsets coming from mod extensions.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), "TicksPerMove")]
	public static class Pawn_TicksPerMove_Patch
	{
		private static bool TryGetLocomotionMovementExtensionMultiplier(Pawn pawn, ref float multiplier)
		{
			LocomotionMovementExtension locomotionMovementExtension =
				pawn.def.GetModExtension<LocomotionMovementExtension>();
			if (locomotionMovementExtension == null)
			{
				return false;
			}

			if (!locomotionMovementExtension.locomotionUrgencies.Contains(CurrentUrgencyUtil.Get(pawn)))
			{
				return false;
			}

			multiplier = locomotionMovementExtension.moveSpeedMultiplier;
			return true;
		}

		private static bool TryGetTerrainTagMovementExtensionMultiplier(Pawn pawn, ref float multiplier)
		{
			TerrainTagMovementExtension terrainTagMovementExtension =
				pawn.def.GetModExtension<TerrainTagMovementExtension>();
			if (terrainTagMovementExtension == null)
			{
				return false;
			}

			if (!terrainTagMovementExtension.Affects(pawn.CurrentTerrainDef()))
			{
				return false;
			}

			multiplier = terrainTagMovementExtension.moveSpeedMultiplier;
			return true;
		}

		public static void Postfix(Pawn __instance, ref int __result)
		{
			float multiplier = 1.0F;
			if (!TryGetLocomotionMovementExtensionMultiplier(__instance, ref multiplier) &&
			    !TryGetTerrainTagMovementExtensionMultiplier(__instance, ref multiplier))
			{
				return;
			}

			__result = Mathf.Clamp(Mathf.RoundToInt(__result / multiplier), 1, 450);
		}
	}
}