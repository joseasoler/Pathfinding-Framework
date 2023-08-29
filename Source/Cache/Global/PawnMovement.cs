using System.Runtime.InteropServices;

namespace PathfindingFramework.Cache.Global
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PawnMovement
	{
		/// <summary>
		/// movementDef.index associated with this pawn.
		/// </summary>
		public ushort movementIndex;

		/// <summary>
		/// True if the pawn cannot pass over fences.
		/// </summary>
		public bool shouldAvoidFences;

		public PawnMovement(ushort index, bool avoidFences)
		{
			movementIndex = index;
			shouldAvoidFences = avoidFences;
		}
	}
}