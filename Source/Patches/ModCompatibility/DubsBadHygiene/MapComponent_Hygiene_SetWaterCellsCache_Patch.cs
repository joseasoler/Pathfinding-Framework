using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.DubsBadHygiene
{
	/// <summary>
	/// SetWaterCellsCache triggers region generation updates very frequently during map generation on maps with water.
	/// When this is tied to the changes in Pathfinding Framework that increase the number of regions in these maps,
	/// this leads to region link inconsistencies. Since the areas used by DBA do not seem to affect region generation in
	/// any way, this patch just prevents region updates when DBA areas are set to dirty.
	/// </summary>
	[HarmonyPatch]
	public static class MapComponent_Hygiene_SetWaterCellsCache_Patch
	{
		private const string TypeName = "MapComponent_Hygiene";
		private const string MethodName = "SetWaterCellsCache";

		public static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.DubsBadHygieneAssembly != null;
		}

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.DubsBadHygieneAssembly, TypeName, MethodName);
		}

		public static void AreaSet(Area area, IntVec3 cell, bool value)
		{
			int index = area.Map.cellIndices.CellToIndex(cell);
			if (area.innerGrid[index] == value)
			{
				return;
			}

			area.innerGrid[index] = value;
			area.Drawer.SetDirty();
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod = AccessTools.Method(typeof(Area), "Set");

			MethodInfo modifiedMethod =
				AccessTools.Method(typeof(MapComponent_Hygiene_SetWaterCellsCache_Patch),
					nameof(AreaSet));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, modifiedMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}