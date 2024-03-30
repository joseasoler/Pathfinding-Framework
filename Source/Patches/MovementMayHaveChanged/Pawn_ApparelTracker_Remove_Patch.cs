using HarmonyLib;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.PawnMovement;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when an apparel with a movement extension is removed.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Remove))]
	public static class Pawn_ApparelTracker_Remove_Patch
	{
		public static void Postfix(Pawn ___pawn, Apparel ap)
		{
			if (___pawn.Spawned && MovementDefDatabase<ThingDef>.Get(ap.def) != null)
			{
				PawnMovementUpdater.Update(___pawn);
			}
		}
	}
}