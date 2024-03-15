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
		private static bool TryGetLocomotionMovementExtensionMultiplier(Pawn pawn, out float multiplier)
		{
			multiplier = 1.0F;

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

		private static bool TryGetTerrainTagMovementExtensionMultiplier(Pawn pawn, out float multiplier)
		{
			multiplier = 1.0F;
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

		public static void Postfix(ref float __result, bool diagonal, Pawn __instance)
		{
			bool applyChanges = false;
			float multiplier = 1.0F;
			if (TryGetLocomotionMovementExtensionMultiplier(__instance, out float locomotionMultiplier))
			{
				applyChanges = true;
				multiplier *= locomotionMultiplier;
			}

			if (TryGetTerrainTagMovementExtensionMultiplier(__instance, out float terrainTagMultiplier))
			{
				applyChanges = true;
				multiplier *= terrainTagMultiplier;
			}

			if (!applyChanges)
			{
				return;
			}

			__result = Mathf.Clamp(__result / multiplier, 1.0F, 450.0F);
		}
	}
}