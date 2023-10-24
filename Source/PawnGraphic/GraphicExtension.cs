using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PathfindingFramework.PawnGraphic
{
	public class GraphicExtension : DefModExtension
	{
		/// <summary>
		/// Contains graphical data for the pawn to use when the condition of the extension is met.
		/// DrawSize and shader will be taken from the pawn's original graphic.
		/// Incompatible with human-like pawns.
		/// </summary>
		public GraphicData bodyGraphicData;

		/// <summary>
		/// Works in a similar way, but for textures defined in the alternateGraphics of the pawn.
		/// </summary>
		public List<GraphicData> alternateGraphics;

		private IEnumerable<string> GraphicDataConfigErrors(string label, GraphicData data)
		{
			if (data.texPath.NullOrEmpty())
			{
				yield return Report.ConfigError(GetType(),
					$"{label} has a null or empty texPath.");
				yield break;
			}

			if (data.drawSize != Vector2.one)
			{
				yield return Report.ConfigError(GetType(),
					$"{label} with texPath {data.texPath} must not define a drawSize; it will be overriden by the pawn's current drawSize.");
			}
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string line in base.ConfigErrors())
			{
				yield return line;
			}

			if (bodyGraphicData == null)
			{
				yield return Report.ConfigError(GetType(), "bodyGraphicData must be defined.");
			}
			else
			{
				foreach (string bodyLine in GraphicDataConfigErrors("bodyGraphicData", bodyGraphicData))
				{
					yield return bodyLine;
				}
			}

			if (alternateGraphics == null)
			{
				yield break;
			}

			for (int alternateIndex = 0; alternateIndex < alternateGraphics.Count; ++alternateIndex)
			{
				string label = $"alternateGraphic {alternateIndex}";
				foreach (string alternateLine in GraphicDataConfigErrors(label, alternateGraphics[alternateIndex]))
				{
					yield return alternateLine;
				}
			}
		}
	}
}