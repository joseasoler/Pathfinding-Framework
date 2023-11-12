using System.Collections.Generic;
using PathfindingFramework.PawnGraphic;
using Verse.AI;

namespace PathfindingFramework
{
	/// <summary>
	/// Applies a graphic change when the pawn is moving at one of the listed locomotion types.
	/// Must be added to the PawnKindDef of the pawn. Incompatible with human-like pawns.
	/// This extension assumes that the PawnKindDef has the same graphics for all of its life stages.
	/// Changes from this extension take precedence over changes from TerrainTagGraphicExtension.
	/// </summary>
	public class LocomotionGraphicExtension : GraphicExtension
	{
		/// <summary>
		/// List of locomotion urgencies that will trigger an effect.
		/// </summary>
		public List<LocomotionUrgency> locomotionUrgencies = new();

		public LocomotionGraphicExtension()
		{
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string line in base.ConfigErrors())
			{
				yield return line;
			}

			if (locomotionUrgencies.Count == 0)
			{
				yield return Report.ConfigError(typeof(LocomotionGraphicExtension),
					"must define at least one locomotion type.");
			}
		}
	}
}