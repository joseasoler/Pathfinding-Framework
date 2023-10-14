using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Patches district generation to take into account the changes from RegionMaker_TryGenerateRegionFrom_Patch.
	/// It adds an additional condition that two regions must follow to belong to the same district.
	/// Besides having the same RegionType, now the TerrainDef() of all regions must be identical.
	/// </summary>
	/// 
	[HarmonyPatch(typeof(RegionTraverser), nameof(RegionTraverser.FloodAndSetDistricts))]
	internal static class RegionTraverser_FloodAndSetDistricts_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return RegionTraverserUtil.PatchRegionMergingPredicate(instructions);
		}
	}
}