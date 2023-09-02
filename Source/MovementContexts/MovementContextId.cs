using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MovementContexts
{
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
			if (pawn.ShouldAvoidFences)
			{
				id += 0x100000000;
			}

			return id;
		}
	}
}