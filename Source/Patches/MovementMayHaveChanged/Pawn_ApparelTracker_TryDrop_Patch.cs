using HarmonyLib;
using PathfindingFramework.PawnMovement;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the pawn movement cache when an apparel with a movement extension is dropped.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TryDrop),
		new[] {typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool)},
		new[] {ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal})]
	public static class Pawn_ApparelTracker_TryDrop_Patch
	{
		public static void Postfix(Pawn ___pawn, Apparel ap)
		{
			if (___pawn.Spawned && ap.def.MovementDef() != null)
			{
				PawnMovementUpdater.Update(___pawn);
			}
		}
	}
}