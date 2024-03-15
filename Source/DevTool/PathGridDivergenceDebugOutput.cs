using System;
using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MovementContexts;
using PathfindingFramework.Patches;
using Verse;
using Verse.AI;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Compares the vanilla path grid with the PF path grid and checks for divergences between both grids.
	/// </summary>
	public static class PathGridDivergenceDebugOutput
	{
		/// <summary>
		/// Generate a path divergence error report. For the parameters, see MovementContextId.
		/// </summary>
		/// <param name="avoidFences">True if the pawn is an animal who must avoid fences.</param>
		private static void CalculatePathGridDivergence(bool avoidFences)
		{
			Map map = Find.CurrentMap;
			MovementContext context = null;
			foreach (MovementContext currentContext in map.MovementContextData().ActiveContexts())
			{
				if (currentContext.MovementDef == MovementDefOf.PF_Movement_Terrestrial &&
				    currentContext.CanIgnoreFire == false &&
				    currentContext.ShouldAvoidFences == avoidFences)
				{
					context = currentContext;
					break;
				}
			}

			if (context == null)
			{
				return;
			}

			PathingContext vanillaPathingContext = avoidFences ? map.pathing.FenceBlocked : map.pathing.Normal;
			int[] vanillaPathGrid = vanillaPathingContext.pathGrid.pathGrid;
			int[] modPathGrid = context.PathingContext.pathGrid.pathGrid;
			int minSize = Math.Min(vanillaPathGrid.Length, modPathGrid.Length);
			List<Tuple<IntVec3, int, int>> divergencesList = new List<Tuple<IntVec3, int, int>>();

			for (int cellIndex = 0; cellIndex < minSize; ++cellIndex)
			{
				int vanilla = vanillaPathGrid[cellIndex];
				int mod = modPathGrid[cellIndex];
				if (vanilla != mod)
				{
					divergencesList.Add(new Tuple<IntVec3, int, int>(map.cellIndices.IndexToCell(cellIndex), vanilla, mod));
				}
			}

			int divergenceCount = divergencesList.Count;
			bool differentLengths = vanillaPathGrid.Length != modPathGrid.Length;
			if (vanillaPathGrid.Length != modPathGrid.Length)
			{
				++divergenceCount;
			}

			string[,] dataTable = new string[3, 1 + divergenceCount];
			dataTable[0, 0] = "Cell";
			dataTable[1, 0] = "Vanilla";
			dataTable[2, 0] = "Modded";

			int dataTableRow = 1;
			if (differentLengths)
			{
				dataTable[0, dataTableRow] = "Cell count";
				dataTable[1, dataTableRow] = vanillaPathGrid.Length.ToString();
				dataTable[2, dataTableRow] = modPathGrid.Length.ToString();
				++dataTableRow;
			}

			foreach (var divergence in divergencesList)
			{
				dataTable[0, dataTableRow] = divergence.Item1.ToString();
				dataTable[1, dataTableRow] = divergence.Item2.ToString();
				dataTable[2, dataTableRow] = divergence.Item3.ToString();
				++dataTableRow;
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}

		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: true)]
		public static void NormalPathGridDivergence()
		{
			CalculatePathGridDivergence(false);
		}
		
		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: true)]
		public static void FencesPathGridDivergence()
		{
			CalculatePathGridDivergence(true);
		}
	}
}