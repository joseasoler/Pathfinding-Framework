using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.PawnGraphic
{
	public static class GraphicLoader
	{
		private static void InitGraphic(GraphicData data)
		{
			if (data != null && !data.texPath.NullOrEmpty())
			{
				data.graphicClass ??= typeof(Graphic_Multi);
				data.ExplicitlyInitCachedGraphic();
			}
		}

		public static void Initialize()
		{
			List<PawnKindDef> allPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
			for (int thingIndex = 0; thingIndex < allPawnKindDefs.Count; ++thingIndex)
			{
				PawnKindDef pawnKindDef = allPawnKindDefs[thingIndex];
				if (pawnKindDef.race?.race == null || pawnKindDef.modExtensions == null)
				{
					continue;
				}

				// GraphicExtension is incompatible with human-like pawns.
				bool isHumanLike = pawnKindDef.race.race.Humanlike;

				for (int extensionIndex = 0; extensionIndex < pawnKindDef.modExtensions.Count; ++extensionIndex)
				{
					DefModExtension extension = pawnKindDef.modExtensions[extensionIndex];
					if (extension is not GraphicExtension graphicExtension)
					{
						continue;
					}

					if (isHumanLike)
					{
						Report.Error(
							$"Human-like PawnKindDef {pawnKindDef.defName} has a {extension.GetType().Name}, which is not supported.");
						continue;
					}

					InitGraphic(graphicExtension.bodyGraphicData);
					if (graphicExtension.alternateGraphics == null)
					{
						if (!pawnKindDef.alternateGraphics.NullOrEmpty())
						{
							Report.Warning(
								$"PawnKindDef {pawnKindDef.defName} has alternate graphics, but its {extension.GetType().Name} does not contain alternate graphics.");
						}

						continue;
					}

					if (pawnKindDef.alternateGraphics.NullOrEmpty())
					{
						Report.Warning(
							$"PawnKindDef {pawnKindDef.defName} does not have alternate graphics, but its {extension.GetType().Name} contains alternate graphics.");
						// Do not initialize the alternate graphics of the extension, they will not be used.
						continue;
					}

					int pawnCount = pawnKindDef.alternateGraphics.Count;
					int extensionCount = graphicExtension.alternateGraphics.Count;
					if (pawnCount != extensionCount)
					{
						Report.Error(
							$"PawnKindDef {pawnKindDef.defName} has {pawnCount} alternate graphic, but its {extension.GetType().Name} contains {extensionCount} alternate graphics. Due to this mismatch, alternate graphics of the extension will not be used.");
						continue;
					}

					for (int alternateIndex = 0; alternateIndex < graphicExtension.alternateGraphics.Count; ++alternateIndex)
					{
						InitGraphic(graphicExtension.alternateGraphics[alternateIndex]);
					}
				}
			}
		}
	}
}