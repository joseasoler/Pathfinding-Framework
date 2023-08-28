using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace PathfindingFramework.Cache.Global;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct PawnMovement(ushort movementIndex, bool shouldAvoidFences)
{
	/// <summary>
	/// movementDef.index associated with this pawn.
	/// </summary>
	public ushort movementIndex = movementIndex;

	/// <summary>
	/// True if the pawn cannot pass over fences.
	/// </summary>
	public bool shouldAvoidFences = shouldAvoidFences;
}