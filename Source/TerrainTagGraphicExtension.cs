using System.Collections.Generic;
using PathfindingFramework.PawnGraphic;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Pawns with this extension can change their graphic depending on the terrain tag their are on.
	/// Must be added to the PawnKindDef of the pawn. Incompatible with human-like pawns.
	/// Not compatible with PawnKindDefs which have different graphics on their life stages.
	/// </summary>
	public class TerrainTagGraphicExtension : GraphicExtension
	{
		/// <summary>
		/// List of terrain tags that will trigger a graphics change.
		/// </summary>
		public List<string> terrainTags = new();

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