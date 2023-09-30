using System;
using System.Collections.Generic;
using System.Text;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.DevTool
{
	public static class PathFinderErrorDebug
	{
		private static IntVec3 _start;
		private static LocalTargetInfo _dest;
		private static TraverseParms _traverseParms;
		private static IntVec3 _current;

		private static List<string> _data = new();
		private static bool _shouldPrint;

		public static void StartEntry(IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParms)
		{
			_start = start;
			_dest = dest;
			_traverseParms = traverseParms;
			_current = IntVec3.Invalid;
			_data = new List<string>();
			_shouldPrint = false;
		}

		public static void EnablePrint(string str)
		{
			_data.Add(str);
			_shouldPrint = true;
		}

		private static string CellToString(IntVec3 cell, Map map)
		{
			if (cell == IntVec3.Invalid)
			{
				return "None";
			}

			return cell.InBounds(map)
				? $"(x:{cell.x},z:{cell.z},terrain:{cell.GetTerrain(map).label})]"
				: $"(x:{cell.x},z:{cell.z},OutOfBounds)]";
		}

		public static void AddPathFinderData(Map map, PriorityQueue<int, int> openList, PathGrid pathGrid, string str)
		{
			string nextElement = "None";
			IntVec3 cell = IntVec3.Invalid;
			if (openList.TryPeek(out int value, out int priority))
			{
				cell = map.cellIndices.IndexToCell(value);
				nextElement = $"{CellToString(cell, map)} -> {priority}";
			}

			_data.Add($"Stage:{str}|Current:{CellToString(_current, map)},Next:{nextElement}|Total:{openList.Count}");

			if (str == "Open cell")
			{
				_current = cell;
			}

			if (str == "Neighbor consideration")
			{
				AddNeighborsData(map, pathGrid);
			}
		}

		private static void AddNeighborsData(Map map, PathGrid pathGrid)
		{
			bool flag2 = _traverseParms.mode != TraverseMode.NoPassClosedDoorsOrWater &&
			             _traverseParms.mode != TraverseMode.PassAllDestroyableThingsNotWater;

			for (int index = 0; index < 8; ++index)
			{
				IntVec3 neighborCell = new IntVec3(_current.x + PathFinder.Directions[index], 0,
					_current.z + PathFinder.Directions[index + 8]);
				string neighBorStr = CellToString(neighborCell, map);
				bool inBounds = neighborCell.InBounds(map);
				bool waterAllows = inBounds && (flag2 || !neighborCell.GetTerrain(map).HasTag("Water"));
				bool walkable = inBounds && pathGrid.WalkableFast(neighborCell);
				_data.Add($"\tneighbor:{neighBorStr}|waterAllows:{waterAllows}|walkable:{walkable}");
			}
		}

		public static void FinishEntry()
		{
			if (!_shouldPrint)
			{
				return;
			}

			Pawn pawn = _traverseParms.pawn;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("PathFinder.Find error");
			string pawnStr = pawn?.GetUniqueLoadID();
			string movementStr = pawn?.MovementDef().label;
			string modeStr = Enum.GetName(typeof(TraverseMode), _traverseParms.mode);
			sb.AppendLine($"Begin:{_start}|End:{_dest}|Mode:{modeStr}|Movement:{movementStr}|Pawn:{pawnStr}");

			sb.AppendLine("----------------------------------");
			for (int index = 0; index < _data.Count; ++index)
			{
				sb.AppendLine(_data[index]);
			}

			Report.Error(sb.ToString());
		}
	}
}