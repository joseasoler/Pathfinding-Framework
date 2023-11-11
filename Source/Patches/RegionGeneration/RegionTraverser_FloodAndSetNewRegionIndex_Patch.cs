using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.RegionGeneration
{
	/// <summary>
	/// Patches region merging to take into account the changes from RegionMaker_TryGenerateRegionFrom_Patch.
	/// Besides having the same RegionType, now the UniqueTerrainDef() of all regions must be identical.
	/// </summary>
	[HarmonyPatch(typeof(RegionTraverser), nameof(RegionTraverser.FloodAndSetNewRegionIndex))]
	internal static class RegionTraverser_FloodAndSetNewRegionIndex_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return RegionMergerUtil.PatchRegionMergingPredicate(instructions);
		}
	}
}