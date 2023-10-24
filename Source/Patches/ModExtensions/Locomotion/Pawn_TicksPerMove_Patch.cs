using HarmonyLib;
using PathfindingFramework.PawnGraphic;
using UnityEngine;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions.Locomotion
{
	/// <summary>
	/// Apply LocomotionExtension move speed stat offset.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), "TicksPerMove")]
	internal static class Pawn_TicksPerMove_Patch
	{
		private static void Postfix(Pawn __instance, ref int __result)
		{
			LocomotionMovementExtension locomotionMovementExtension =
				__instance.def.GetModExtension<LocomotionMovementExtension>();
			if (locomotionMovementExtension == null)
			{
				return;
			}

			if (!locomotionMovementExtension.locomotionUrgencies.Contains(CurrentUrgencyUtil.Get(__instance)))
			{
				return;
			}

			__result = Mathf.Clamp(Mathf.RoundToInt(__result / locomotionMovementExtension.moveSpeedMultiplier), 1, 450);
		}
	}
}