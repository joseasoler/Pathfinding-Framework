using System.Collections.Generic;
using PathfindingFramework.DevTool;
using PathfindingFramework.MapPathCosts;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.MovementContexts
{
	public class MovementContextData : MapGrid
	{
		/// <summary>
		/// Contexts identified by their MovementContextId long value.
		/// </summary>
		private readonly Dictionary<long, WeakReference<MovementContext>> _contexts;

		/// <summary>
		/// Create an instance of the cache for a specific map.
		/// </summary>
		public MovementContextData(Map map) : base(map)
		{
			_contexts = new Dictionary<long, WeakReference<MovementContext>>();
		}

		/// <summary>
		/// Update movement context data when the movement type of a pawn changes.
		/// It will try to reuse a existing object if possible, otherwise a new one is created.
		/// </summary>
		/// <param name="pawn">Pawn with updated movement.</param>
		public void UpdatePawn(Pawn pawn)
		{
			MovementContext context = null;
			long movementContextId = MovementContextId.From(pawn);
			if (_contexts.TryGetValue(movementContextId, out WeakReference<MovementContext> contextReference) &&
			    contextReference.IsAlive)
			{
				// Reuse an existing context.
				context = contextReference.Target;
			}
			else
			{
				// Create a new context and update its path grid.
				MovementDef movementDef = pawn.MovementDef();
				context = new MovementContext(movementDef, pawn.Map, !movementDef.penAnimalsDisallowed && pawn.ShouldAvoidFences);
				_contexts[movementContextId] = new WeakReference<MovementContext>(context);
				for (int cellIndex = 0; cellIndex < GridSize; ++cellIndex)
				{
					MapPathCost mapPathCost = Map.MapPathCostGrid().Get(cellIndex);
					context.UpdateCell(cellIndex, mapPathCost);
				}
			}

			pawn.MovementContext() = context;
		}

		/// <summary>
		/// Movement contexts that are currently active in this map.
		/// Exposed to be used in the inspector drawer.
		/// </summary>
		/// <returns>List of active contexts.</returns>
		public List<MovementContext> ActiveContexts()
		{
			List<MovementContext> validContexts = new List<MovementContext>();
			foreach (var entry in _contexts)
			{
				if (!entry.Value.IsAlive)
				{
					continue;
				}

				validContexts.Add(entry.Value.Target);
			}

			return validContexts;
		}

		/// <summary>
		/// Update cell path costs in every movement context.
		/// </summary>
		/// <param name="cell">Cell being updated.</param>
		public void UpdateCell(IntVec3 cell)
		{
			int cellIndex = ToIndex(cell);
			MapPathCost mapPathCost = Map.MapPathCostGrid().Get(cellIndex);
			List<MovementContext> validContexts = ActiveContexts();

			foreach (MovementContext context in validContexts)
			{
				context.UpdateCell(cellIndex, mapPathCost);
			}
		}

		public void UpdateAll()
		{
			List<MovementContext> validContexts = ActiveContexts();
			for (int cellIndex = 0; cellIndex < GridSize; ++cellIndex)
			{
				MapPathCost mapPathCost = Map.MapPathCostGrid().Get(cellIndex);
				foreach (MovementContext context in validContexts)
				{
					context.UpdateCell(cellIndex, mapPathCost);
				}
			}
		}

		public List<MemoryUsageData> MemoryReport()
		{
			List<MemoryUsageData> report = new List<MemoryUsageData>();

			int pathGridCount = GridSize * sizeof(int);
			List<MovementContext> validContexts = ActiveContexts();

			foreach (MovementContext context in validContexts)
			{
				string label = context.MovementDef.LabelCap;
				string grid = context.ShouldAvoidFences ? $"{label} (fences) context" : $"{label} context";
				report.Add(new MemoryUsageData(nameof(MovementContextData), Map.GetUniqueLoadID(),
					grid, pathGridCount));
			}

			return report;
		}
	}
}