using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Parse;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Patches Region.Allows to make it aware of different movement types.
	/// Most calls to RegionTraverser functions have a RegionEntryPredicate which uses Region.Allows. This means that in
	/// almost every case, this patch is enough to ensure movement type aware region pathfinding.
	/// It also means that there will be different cases lurking in the code base that will require explicit patching.
	///
	/// This patch allows pawns to enter impassable regions if the region terrain is passable for them.
	/// Regions composed of impassable terrains in vanilla, and regions that might not be safe for certain movement types
	/// are guaranteed to meet the following two conditions, thanks to RegionMaker_FloodFillAndAddCells_Patch and
	/// RegionMaker_TryGenerateRegionFrom_Patch.
	///
	/// * The entire region is composed of cells that have the same TerrainDef.
	/// * The region has a Prepatcher TerrainDef() field which is set to the mentioned TerrainDef.
	/// * The region has a RegionType of Normal (passable) as long as at least one movement type can traverse it.
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
		/// <param name="isDestination">True if the region is the destination region.</param>
		/// <returns>False if the pawn should avoid this region.</returns>
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
			TerrainDef regionTerrainDef = region.UniqueTerrainDef();
			if (pawn == null)
			{
				// When Region.Allows is called without a pawn, assume that the caller wants a result as similar to vanilla as
				// possible. Disallow regions which have a TerrainDef value which is not passable.
				return regionTerrainDef == null || regionTerrainDef.passability != Traversability.Impassable;
			}

			// Regions with a valid TerrainDef might be impassable for the vanilla movement type.
			// In vanilla, these regions would have not been passable, but as mentioned above, Pathfinding Framework sets them
			// as passable. This means that this function needs to take care of preventing entrance in this case.
			// TerrainDef can also be set for regions which are passable for the vanilla movement type. In this case,
			// It means that the terrain is passable for at least one movement type which is using defaultCost to set most
			// terrains to Unsafe.
			// The last option is that TerrainDef is null, which is the default case for most passable terrains. In this last
			// case, grabbing the terrain from any cell in the region is equivalent, as all of them will provide the same 
			// passability result.
			// See RegionTypeExtended for even more details.
			Map map = pawn.Map;

			// Pawns are not allowed to move through unsafe terrain, with just an exception. Is the pawn is currently on
			// unsafe terrain, they can move to safe terrain.
			IntVec3 startCell = pawn.Position;
			TerrainDef startTerrainDef = startCell.GetTerrain(map);
			bool currentlyOnUnsafeTerrain = pawn.MovementDef().PathCosts[startTerrainDef.MovementIndex()] == PathCost.Unsafe.cost;

			return LocationFinding.IsPassableRegion(region, pawn.MovementDef(), map, isDestination, currentlyOnUnsafeTerrain);
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
					// into the movement type aware passability check function defined above.
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldarg_1); // tp
					yield return new CodeInstruction(OpCodes.Ldarg_2); // isDestination
					yield return new CodeInstruction(OpCodes.Call, movementTypePassableMethod);
				}
			}
		}
	}
}