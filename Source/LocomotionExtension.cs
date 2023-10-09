using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PathfindingFramework
{
	/// <summary>
	/// ToDo: Implementation
	/// </summary>
	public class LocomotionExtension : DefModExtension
	{
		public List<LocomotionUrgency> locomotionUrgencies = new List<LocomotionUrgency>();

		public float moveSpeedMultiplier;

		public GraphicData graphicData;

		public LocomotionExtension()
		{
		}
	}
}