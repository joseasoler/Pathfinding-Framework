using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MovementContexts
{
	/// <summary>
	/// Calculates unique identifiers for a movement context.
	/// Used to determine which MovementContext should be assigned to a pawn.
	/// </summary>
	public static class MovementContextId
	{
		public static long From(Pawn pawn)
		{
			MovementDef movementDef = pawn.MovementDef();
			if (movementDef == null)
			{
				return long.MinValue;
			}

			long id = movementDef.index;
			if (MovementContextUtil.ShouldAvoidFences(pawn))
			{
				id += 0x100000000;
			}

			if (MovementContextUtil.CanIgnoreFire(pawn))
			{
				id += 0x200000000;
			}

			return id;
		}
	}
}