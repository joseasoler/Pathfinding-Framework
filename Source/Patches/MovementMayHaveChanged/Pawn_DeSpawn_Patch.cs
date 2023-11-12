using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Remove the pawn from the PawnMovementCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
	public static class Pawn_DeSpawn_Patch
	{
		public static void Postfix(Pawn __instance)
		{
			__instance.MovementContext() = null;
		}
	}
}