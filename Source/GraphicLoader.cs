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
				Report.Error($"Enqueueing the initialization of a graphic afater game load: {data}");
				return;
			}

			_graphicsToInitialize.Add(data);
		}

		public static void Initialize()
		{
			for (int index = 0; index < _graphicsToInitialize.Count; ++index)
			{
				GraphicData data = _graphicsToInitialize[index];
				data.graphicClass ??= typeof(Graphic_Multi);
				data.ExplicitlyInitCachedGraphic();
			}

			_graphicsToInitialize = null;
		}
	}
}