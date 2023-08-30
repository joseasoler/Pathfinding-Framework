using System.Runtime.CompilerServices;

namespace PathfindingFramework.Cache.Global
{
	/// <summary>
	/// Utility class to pack different pawn movement parameters into a single integer. This integer has the following
	/// memory layout: 0x0000AFMI
	/// Where AF is an avoid fences boolean values, and MI is a MovementDef.index.
	/// </summary>
	public static class PawnMovement
	{
		/// <summary>
		/// Movement index is packed into the lowest 2 byte of the integer.
		/// </summary>
		private const int MovementIndexMask = 0xFF;

		/// <summary>
		/// Mask over the position used by the avoid fences mask.
		/// </summary>
		private const int AvoidFencesMask = 0b100000000;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FromParameters(int movementIndex, bool avoidFences)
		{
			return movementIndex + (avoidFences ? AvoidFencesMask : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MovementIndex(int pawnMovement)
		{
			return pawnMovement & MovementIndexMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AvoidFences(int pawnMovement)
		{
			return (pawnMovement & AvoidFencesMask) != 0;
		}
	}
}