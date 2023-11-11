using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.ModExtensions
{
	/// <summary>
	/// Update graphics when a creature dies.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.SetDead))]
	public class Pawn_HealthTracker_SetDead_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			___pawn.GraphicContext()?.Death();
		}
	}
}