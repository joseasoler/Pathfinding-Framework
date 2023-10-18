using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	[HarmonyPatch(typeof(RegionCostCalculator), "GetPreciseRegionLinkDistances")]
	internal static class RegionCostCalculator_GetPreciseRegionLinkDistances_Patch
	{
		private static RegionLink _regionLink = null;

		private static int count = 0;

		internal static void Prefix()
		{
			_regionLink = null;
		}

		internal static void EnableErrors(RegionLink link)
		{
			_regionLink = link;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo logErrorOnceMethod =
				AccessTools.Method(typeof(Log), nameof(Log.ErrorOnce), new[] { typeof(string), typeof(int) });
			MethodInfo enableErrorsMethod =
				AccessTools.Method(typeof(RegionCostCalculator_GetPreciseRegionLinkDistances_Patch), nameof(EnableErrors));

			object linkOperand = null;
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (linkOperand == null && instruction.opcode == OpCodes.Stloc_S)
				{
					linkOperand = instruction.operand;
				}

				if (instruction.Calls(logErrorOnceMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_S, linkOperand);
					yield return new CodeInstruction(OpCodes.Call, enableErrorsMethod);
				}
			}
		}

		internal static void Postfix(Region region, TraverseParms ___traverseParms, IntVec3 ___destinationCell,
			Dictionary<RegionLink, IntVec3> ___linkTargetCells, Dictionary<int, float> ___tmpDistances)
		{
			if (_regionLink == null || count > 10)
			{
				return;
			}

			Pawn pawn = ___traverseParms.pawn;
			Map map = pawn?.Map;
			string pawnName = pawn != null ? pawn.GetUniqueLoadID() : "No pawn";
			string startCellStr = pawn != null ? pawn.Position.ToString() : "Unknown";
			string startRegionStr = pawn != null ? pawn.Position.GetRegion(pawn.Map).id.ToString() : "Unknown";

			string destCellStr = ___destinationCell.ToString();
			string destRegionStr = region != null ? region.id.ToString() : "None";

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(
				$"RegionCostCalculator error (Dijkstra) detected: {pawnName} moving from {startCellStr}[region:{startRegionStr}] to {destCellStr}[region:{destRegionStr}]");
			sb.AppendLine(
				$"While processing link {_regionLink.span.ToString()} between regions {_regionLink.RegionA.id}[cell:{_regionLink.RegionA.AnyCell}] and  {_regionLink.RegionB.id}[cell:{_regionLink.RegionB.AnyCell}]");

			if (___linkTargetCells.TryGetValue(_regionLink, out IntVec3 cell))
			{
				if (map != null && ___tmpDistances.TryGetValue(map.cellIndices.CellToIndex(cell), out float distance))
				{
					sb.AppendLine($"Targeting cell [{cell.x}, {cell.z}] at a distance of {distance}");
				}
				else
				{
					sb.AppendLine($"Targeting cell [{cell.x}, {cell.z}]. Distance could not be found!");
				}
			}
			else
			{
				sb.AppendLine($"Targeting unknown cell.");
			}

			if (map != null)
			{
				List<string> distances = new List<string>();
				foreach (var entry in ___tmpDistances)
				{
					IntVec3 currentCell = map.cellIndices.IndexToCell(entry.Key);
					distances.Add($"[{currentCell.x}, {currentCell.z}] -> {entry.Value}");
				}

				distances.Sort();
				sb.AppendLine($"Full distance map:");
				foreach (string logEntry in distances)
				{
					sb.AppendLine(logEntry);
				}
			}

			Report.Error(sb.ToString());

			// Update for next error case.
			_regionLink = null;
			++count;
		}
	}
}