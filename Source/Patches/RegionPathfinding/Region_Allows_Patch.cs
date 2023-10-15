using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Parse;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Movement type aware region impassability.
	/// Most calls to RegionTraverser functions have a RegionEntryPredicate which uses Region.Allows.
	/// The following patch allows pawns to enter supposedly impassable regions if they have a terrain that is passable
	/// for them.
	/// Formerly impassable regions are guaranteed to have a unique TerrainDef thanks to
	/// RegionMaker_FloodFillAndAddCells_Patch. This TerrainDef is set using Prepatcher fields in
	/// RegionMaker_TryGenerateRegionFrom_Patch. The same patch also sets the type of these regions to be passable.
	/// </summary>
	[HarmonyPatch(typeof(Region), nameof(Region.Allows))]
	public class Region_Allows_Patch
	{
		/// <summary>
		/// Pawn movement aware version of RegionTypeUtility.Passable.
		/// </summary>
		/// <param name="regionTypePassable">Result of the vanilla check.</param>
		/// <param name="region">Region to check.</param>
		/// <param name="parms">Traverse parameters of this region pathfinding check.</param>
		/// <param name="isDestination">True if the region is the destionation region.</param>
		/// <returns>False if the current pawn should not be able to enter this region.</returns>
		private static bool MovementTypePassable(bool regionTypePassable, Region region, TraverseParms parms,
			bool isDestination)
		{
			// Since RegionMaker_TryGenerateRegionFrom_Patch sets a TerrainDef to impassable regions with a terrain passable
			// for at least one movement type and then sets the region as passable, if vanilla returns impassable then the
			// region is definitely impassable for every movement type.
			if (!regionTypePassable)
			{
				return false;
			}

			Pawn pawn = parms.pawn;
			TerrainDef regionTerrainDef = region.TerrainDef();
			if (pawn == null)
			{
				// When Region.Allows is called without a pawn, assume that the caller wants a result as similar to vanilla as
				// possible. Since regions with a TerrainDef() are impassable with vanilla, disallow moving through them.
				return regionTerrainDef == null;
			}

			// Regions with a valid TerrainDef field would be impassable in vanilla, but they have been made passable because
			// at least one movement type can traverse them.
			// If the region does not have a valid TerrainDef, grab the terrain from any of its cells. This will be enough to
			// make aquatic creatures avoid land.
			Map map = pawn.Map;
			regionTerrainDef ??= region.AnyCell.GetTerrain(map);

			// Pawns are not allowed to move through unsafe terrain, with just an exception. Is the pawn is currently on
			// unsafe terrain, they can move to safe terrain.
			IntVec3 startCell = pawn.Position;
			TerrainDef startTerrainDef = startCell.GetTerrain(map);
			bool currentOnUnsafeTerrain = pawn.MovementDef().PathCosts[startTerrainDef.index] == PathCost.Unsafe.cost;

			// If the pawn is on unsafe terrain, allow them to traverse unsafe terrain regions as long as the destination
			// is safe.
			short nonTraversablePathCost =
				isDestination || !currentOnUnsafeTerrain ? PathCost.Unsafe.cost : PathCost.Impassable.cost;
			short pathCost = pawn.MovementDef().PathCosts[regionTerrainDef.index];
			return pathCost < nonTraversablePathCost;
		}

		/// <summary>
		/// Make the RegionTypeUtility.Passable check aware of the current pawn movement type.
		/// </summary>
		/// <param name="instructions">Original instructions.</param>
		/// <returns>New instructions.</returns>
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo movementTypePassableMethod =
				AccessTools.Method(typeof(Region_Allows_Patch), nameof(MovementTypePassable));

			MethodInfo regionTypePassableMethod =
				AccessTools.Method(typeof(RegionTypeUtility), nameof(RegionTypeUtility.Passable));
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo info &&
				    info == regionTypePassableMethod)
				{
					// Instead of using RegionTypeUtility.Passable directly, feed its result along with additional parameters
					// into the movement type aware passability check function.
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldarg_1); // tp
					yield return new CodeInstruction(OpCodes.Ldarg_2); // isDestination
					yield return new CodeInstruction(OpCodes.Call, movementTypePassableMethod);
				}
			}
		}
	}
}