using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapComponents.MapPathCosts;
using PathfindingFramework.MapComponents.MovementContexts;
using Verse;

namespace PathfindingFramework.MapComponents
{
	public class PathfindingInformation : MapComponent
	{
		private MapPathCostGrid _costGrid;
		private MovementContextData _contextData;

		public PathfindingInformation(Map map) : base(map)
		{
			if (map.uniqueID < 0)
			{
				// m00nl1ght.MapPreview uses maps without uniqueID to generate previews.
				return;
			}

			_costGrid = new MapPathCostGrid(map);
			// Start accepting pathfinding update calls.
			_costGrid = new MapPathCostGrid(map);
			map.MapPathCostGrid() = _costGrid;
			_contextData = new MovementContextData(map);
			map.MovementContextData() = _contextData;
		}

		public override void FinalizeInit()
		{
			// Set up the cost grid.
			_costGrid.UpdateSnowAllCells();
			foreach (IntVec3 cell in map.AllCells)
			{
				_costGrid.UpdateThings(cell);
			}

			// Initialize movement context handling after the map is fully created or loaded.
			_contextData.UpdateAllCells();
		}

		public override void MapRemoved()
		{
			map.MapPathCostGrid() = null;
			map.MovementContextData() = null;
		}
	}
}