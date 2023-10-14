using System.Collections.Generic;
using Verse;

namespace PathfindingFramework
{
	public static class GraphicLoader
	{
		public static void Initialize()
		{

			List<ThingDef> allThings = DefDatabase<ThingDef>.AllDefsListForReading;
			for (int thingIndex = 0; thingIndex < allThings.Count; ++thingIndex)
			{
				ThingDef thing = allThings[thingIndex];
				if (thing.race == null || thing.modExtensions == null)
				{
					continue;
				}

				for (int extensionIndex = 0; extensionIndex < thing.modExtensions.Count; ++extensionIndex)
				{
					DefModExtension extension = thing.modExtensions[extensionIndex];
					GraphicData data = null;
					if (extension is LocomotionExtension locomotionExtension)
					{
						data = locomotionExtension.graphicData;
					}
					else if (extension is TerrainTagGraphicExtension terrainTagGraphicExtension)
					{
						data = terrainTagGraphicExtension.graphicData;
					}

					if (data == null || data.texPath.NullOrEmpty())
					{
						continue;
					}

					data.graphicClass ??= typeof(Graphic_Multi);
					data.ExplicitlyInitCachedGraphic();
				}
			}
		}
	}
}