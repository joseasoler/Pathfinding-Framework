using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	[HarmonyPatch(typeof(RegionCostCalculator), "GetPreciseRegionLinkDistances")]
	internal static class RegionCostCalculator_GetPreciseRegionLinkDistances_Patch
	{
		private static bool _shouldLogError = false;

		private static int _regionLinkIndex;

		internal static void Prefix()
		{
			_shouldLogError = false;
		}

		internal static void EnableErrors(int index)
		{
			_shouldLogError = true;
			_regionLinkIndex = index;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo logErrorOnceMethod =
				AccessTools.Method(typeof(Log), nameof(Log.ErrorOnce), new[] { typeof(string), typeof(int) });
			MethodInfo enableErrorsMethod =
				AccessTools.Method(typeof(RegionCostCalculator_GetPreciseRegionLinkDistances_Patch), nameof(EnableErrors));

			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (instruction.Calls(logErrorOnceMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_3);
					yield return new CodeInstruction(OpCodes.Call, enableErrorsMethod);
				}
			}
		}

		internal static void Postfix(Region region, TraverseParms ___traverseParms, IntVec3 ___destinationCell,
			Dictionary<RegionLink, IntVec3> ___linkTargetCells)
		{
			if (!_shouldLogError)
			{
				return;
			}

			Pawn pawn = ___traverseParms.pawn;
			string pawnName = pawn != null ? pawn.Name.ToString() : "No pawn";
			string originStr = pawn != null ? pawn.Position.ToString() : "Unknown";
			string destinationStr = ___destinationCell.ToString();

			string regionStr = "None";
			string linkStr = "None";
			string linkTargetCellStr = "None";
			if (region != null)
			{
				regionStr = region.id.ToString();
				if (region.links.Count < _regionLinkIndex && _regionLinkIndex > 0)
				{
					RegionLink link = region.links[_regionLinkIndex];
					linkStr = link.ToString();
					if (___linkTargetCells.TryGetValue(link, out var cell))
					{
						linkTargetCellStr = cell.ToString();
					}
				}
			}

			Report.Error(
				$"Additional RegionCostCalculator logging (Dijkstra): {pawnName} moving from {originStr} to {destinationStr}. Region: {regionStr}, region link: {linkStr}, region link target: {linkTargetCellStr}");
			_shouldLogError = false;
		}
	}
}