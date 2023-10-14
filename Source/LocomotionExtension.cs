using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework
{
	/// <summary>
	/// When the pawn is moving at one of the listed locomotion types, it is possible to apply a movement speed multiplier
	/// or a graphics change.
	/// </summary>
	public class LocomotionExtension : DefModExtension
	{
		/// <summary>
		/// List of locomotion urgencies that will trigger an effect.
		/// </summary>
		public List<LocomotionUrgency> locomotionUrgencies = new List<LocomotionUrgency>();

		/// <summary>
		/// Movement speed multiplier to apply when moving at one of the listed locomotions. 
		/// </summary>
		public float moveSpeedMultiplier = 1.0F;

		/// <summary>
		/// Graphic to use when moving at one of the listed locomotions.
		/// DrawSize and shader will be taken from the pawn's original graphic. 
		/// </summary>
		public GraphicData graphicData;

		public LocomotionExtension()
		{
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (graphicData != null && graphicData.texPath.NullOrEmpty())
			{
				yield return Report.ConfigError(typeof(LocomotionExtension), "graphicData has a null or empty texPath.");
			}
		}
	}
}