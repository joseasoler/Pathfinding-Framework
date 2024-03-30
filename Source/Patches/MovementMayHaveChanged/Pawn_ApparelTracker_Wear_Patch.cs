using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.PawnMovement;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when an apparel with a movement extension is equipped.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
	public static class Pawn_ApparelTracker_Wear_Patch
	{
		public static void Postfix(Pawn ___pawn, Apparel newApparel)
		{
			if (___pawn.Spawned && MovementDefDatabase<ThingDef>.Get(newApparel.def) != null)
			{
				PawnMovementUpdater.Update(___pawn);
			}
		}
	}
}