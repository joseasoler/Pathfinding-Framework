using System;
using System.Collections.Generic;
using PathfindingFramework.MovementContexts;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Compares the vanilla path grid with the PF path grid and checks for divergences between both grids.
	/// </summary>
	public static class PathGridDivergenceDebugOutput
	{
		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: false)]
		public static void PathGridDivergence()
		{
			Map map = Find.CurrentMap;

			MovementContext context = map?.MovementContextData().ActiveContexts()
				.Find(context => context.MovementDef == MovementDefOf.PF_Movement_Terrestrial);

			if (context == null)
			{
				return;
			}

			int[] vanillaPathGrid = map.pathing.Normal.pathGrid.pathGrid;
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

			var dataTable = new string[3, 1 + divergenceCount];
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
	}
}