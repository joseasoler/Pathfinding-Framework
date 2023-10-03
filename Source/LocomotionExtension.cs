using System.Collections.Generic;
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

		public List<StatModifier> statOffsets = new List<StatModifier>();
		
		public GraphicData graphicData;

		public LocomotionExtension()
		{
		}
	}
}