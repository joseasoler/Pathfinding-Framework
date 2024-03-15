using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MovementContexts
{
	/// <summary>
	/// Utility functions used by the movement context feature.
	/// </summary>
	public static class MovementContextUtil
	{
		/// <summary>
		/// A pawn should avoid fences if it should naturally avoid fences, its movement def allows pen animals, and it
		/// does not have a job that allows bashing fences.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>True if this pawn should avoid fences.</returns>
		public static bool ShouldAvoidFences(Pawn pawn)
		{
			return !pawn.MovementDef().penAnimalsDisallowed && pawn.ShouldAvoidFences &&
				(pawn.CurJob == null || !pawn.CurJob.canBashFences);
		}

		/// <summary>
		/// Returns true if a pawn is inherently non-flammable.
		/// At the moment this ignores apparel, genes and other similar factors. Only the PawnKindDef is considered.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>True if this pawn can ignore the path costs of fire.</returns>
		public static bool CanIgnoreFire(Pawn pawn)
		{
			return pawn.def.BaseFlammability <= 0;
		}
	}
}