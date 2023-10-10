using System.Collections.Generic;
using Verse;

namespace PathfindingFramework
{
	public static class GraphicLoader
	{
		private static List<GraphicData> _graphicsToInitialize = new List<GraphicData>();

		public static void InitializeWhenLoadingFinished(GraphicData data)
		{
			if (_graphicsToInitialize == null)
			{
				Report.Error($"Enqueueing the initialization of a graphic after game is done loading: {data}");
				return;
			}
			else if (data == null)
			{
				Report.Error($"Enqueueing null graphic for initialization!");
				return;
			}

			_graphicsToInitialize.Add(data);
		}

		public static void Initialize()
		{
			if (_graphicsToInitialize == null)
			{
				Report.Error("The list of graphics to initialize is null. This should never happen.");
				return;
			}

			for (int index = 0; index < _graphicsToInitialize.Count; ++index)
			{
				GraphicData data = _graphicsToInitialize[index];
				if (data == null)
				{
					Report.Error("Null graphic found in the queue of graphics to initialize.");
					continue;
				}

				data.graphicClass ??= typeof(Graphic_Multi);
				data.ExplicitlyInitCachedGraphic();
			}

			_graphicsToInitialize = null;
		}
	}
}