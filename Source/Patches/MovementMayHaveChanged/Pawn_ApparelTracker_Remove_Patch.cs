﻿using HarmonyLib;
using PathfindingFramework.Cache.Global;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when an apparel with a movement extension is removed.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Remove))]
	internal static class Pawn_ApparelTracker_Remove_Patch
	{
		internal static void Postfix(Pawn ___pawn, Apparel ap)
		{
			if (___pawn.Spawned && MovementExtensionCache.Contains(ap.def))
			{
				PawnMovementCache.Update(___pawn);
			}
		}
	}
}