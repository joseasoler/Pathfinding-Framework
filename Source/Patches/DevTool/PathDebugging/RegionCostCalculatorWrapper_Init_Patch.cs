using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using PathfindingFramework.ErrorHandling;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/// <summary>
	/// Logs additional information when the dreaded "Couldn't find any destination regions." error appears.
	/// </summary>
	[HarmonyPatch(typeof(RegionCostCalculatorWrapper), nameof(RegionCostCalculatorWrapper.Init))]
	internal static class RegionCostCalculatorWrapper_Init_Patch
	{
		private static void LogCouldNotFindDestinationError(string error, Map map, CellRect end,
			TraverseParms traverseParms,
			List<int> disallowedCorners)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(error);

			Pawn pawn = traverseParms.pawn;
			string pawnStr = pawn != null ? pawn.GetUniqueLoadID() : "None";
			string startStr = pawn != null ? $"[{pawn.Position.x}, {pawn.Position.z}]" : "Unknown";
			string startTerrain = pawn != null && pawn.Position.InBounds(map)
				? pawn.Position.GetTerrain(map).defName
				: "Unknown";
			string traverseModeStr = Enum.GetName(typeof(TraverseMode), traverseParms.mode);
			sb.AppendLine(
				$"{pawnStr} requested the region cost calculation. Starting point: {startStr} with terrain {startTerrain}. Traverse mode: {traverseModeStr}.");

			HashSet<IntVec3> destinationCells = new HashSet<IntVec3>();
			if (end.Width == 1 && end.Height == 1)
			{
				sb.AppendLine($"Destination rectangle with single cell.");
				destinationCells.Add(end.CenterCell);
			}
			else
			{
				sb.Append(
					$"Destination rectangle from [{end.minX}, {end.minZ}] to [{end.maxX}, {end.maxZ}] with disallowed corners: ");
				foreach (int cornerIndex in disallowedCorners)
				{
					IntVec3 cornerCell = map.cellIndices.IndexToCell(cornerIndex);
					sb.Append($"[{cornerCell.x}, {cornerCell.z}], ");
				}

				sb.AppendLine();
				foreach (IntVec3 destination in end)
				{
					destinationCells.Add(destination);
				}
			}

			foreach (IntVec3 dest in destinationCells)
			{
				bool inBounds = dest.InBounds(map);
				string terrainDef = inBounds ? dest.GetTerrain(map).defName : "None";
				bool isDisallowed = inBounds && disallowedCorners.Contains(map.cellIndices.CellToIndex(dest));
				Region region = inBounds && !isDisallowed ? dest.GetRegion(map) : null;
				string regionId = region != null ? region.id.ToString() : "None";
				bool allows = region != null && region.Allows(traverseParms, true);
				sb.AppendLine(
					$"\tCell:[{dest.x}, {dest.z}, {terrainDef}], disallowed corner: {isDisallowed}, region: {regionId}, allows: {allows}");
			}

			sb.AppendLine("Additional error report information.");
			sb.AppendLine(ErrorReport.Get(map));
			Report.Error(sb.ToString());
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo logErrorMethod = AccessTools.Method(typeof(Log), nameof(Log.Error), new[] { typeof(string) });
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(logErrorMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld,
						AccessTools.Field(typeof(RegionCostCalculatorWrapper), "map"));
					yield return new CodeInstruction(OpCodes.Ldarg_1); // end
					yield return new CodeInstruction(OpCodes.Ldarg_2); // traverseParms
					const byte disallowedCornersIndex = 8;
					yield return new CodeInstruction(OpCodes.Ldarg_S, disallowedCornersIndex); // disallowedCorners
					yield return new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(RegionCostCalculatorWrapper_Init_Patch),
							nameof(LogCouldNotFindDestinationError)));
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}