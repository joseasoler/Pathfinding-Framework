using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	public class TerrainTagGraphicExtension : DefModExtension
	{
		public List<string> terrainTags = new();

		public GraphicData graphicData;

		public TerrainTagGraphicExtension()
		{
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (terrainTags.NullOrEmpty())
			{
				yield return Report.ConfigError(typeof(TerrainTagGraphicExtension), "terrainTags must not be empty.");
			}

			if (graphicData == null)
			{
				yield return Report.ConfigError(typeof(TerrainTagGraphicExtension), "graphicData must be defined.");
			}
			else
			{
				if (graphicData.texPath.NullOrEmpty())
				{
					yield return Report.ConfigError(typeof(TerrainTagGraphicExtension),
						"graphicData has a null or empty texPath.");
				}
				else
				{
					if (graphicData.drawSize != Vector2.one)
					{
						yield return Report.ConfigError(typeof(TerrainTagGraphicExtension),
							"graphicData should not define a drawSize, as this will be overriden by the pawn's current drawSize.");
					}

					GraphicLoader.InitializeWhenLoadingFinished(graphicData);
				}
			}
		}

		public bool Affects(TerrainDef terrainDef)
		{
			if (terrainDef?.tags == null)
			{
				return false;
			}

			List<string> terrainDefTags = terrainDef.tags;
			for (int index = 0; index < terrainDefTags.Count; ++index)
			{
				if (terrainTags.Contains(terrainDefTags[index]))
				{
					return true;
				}
			}

			return false;
		}
	}
}