using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework
{
	public class LocomotionExtension : DefModExtension
	{
		public List<LocomotionUrgency> locomotionUrgencies = new List<LocomotionUrgency>();

		public float moveSpeedMultiplier;

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

			if (graphicData != null)
			{
				if (graphicData.texPath.NullOrEmpty())
				{
					yield return Report.ConfigError(typeof(LocomotionExtension), "graphicData has a null or empty texPath.");
				}
				else
				{
					GraphicLoader.InitializeWhenLoadingFinished(graphicData);
				}
			}
		}

	}
}