using HarmonyLib;
using UnityEngine;
using Verse;
using Verse.AI;

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
			LocomotionExtension locomotionExtension = __instance.LocomotionExtension();
			if (locomotionExtension == null)
			{
				return;
			}

			if (!locomotionExtension.locomotionUrgencies.Contains(CurrentUrgency_Util.Get(__instance)))
			{
				return;
			}

			__result = Mathf.Clamp(Mathf.RoundToInt(__result / locomotionExtension.moveSpeedMultiplier), 1, 450);
		}
	}
}