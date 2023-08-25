using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.Cache;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.DisplayMovementInStats
{
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpecialDisplayStats))]
	internal static class Pawn_SpecialDisplayStats_Patch
	{
		internal static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> values, Pawn __instance)
		{
			foreach (var value in values)
			{
				yield return value;
			}

			var movementIndex = PawnMovementCache.Get(__instance);
			var movementDef = DefDatabase<MovementDef>.AllDefsListForReading[movementIndex];
			yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "PF_Locomotion".Translate(),
				movementDef.LabelCap, "PF_LocomotionReportText".Translate(), 2501);
		}
	}
}