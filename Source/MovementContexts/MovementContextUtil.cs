using Verse;

namespace PathfindingFramework.MovementContexts
{
	public static class MovementContextUtil
	{
		public static bool CanIgnoreFire(Pawn pawn)
		{
			return pawn.def.BaseFlammability <= 0;
		}
	}
}