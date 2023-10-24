using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PathfindingFramework
{
	/// <summary>
	/// Applies a movement speed multiplier when the pawn is moving at one of the listed locomotion types.
	/// This extension must be applied to the ThingDef of the pawn.
	/// </summary>
	public class LocomotionMovementExtension : DefModExtension
	{
		/// <summary>
		/// List of locomotion urgencies that will trigger an effect.
		/// </summary>
		public List<LocomotionUrgency> locomotionUrgencies = new();

		/// <summary>
		/// Movement speed multiplier to apply when moving at one of the listed locomotions.
		/// </summary>
		public float moveSpeedMultiplier = 1.0F;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (locomotionUrgencies.Count == 0)
			{
				yield return Report.ConfigError(typeof(LocomotionMovementExtension),
					"must define at least one locomotion type.");
			}

			if (Math.Abs(moveSpeedMultiplier - 1.0F) < 0.00001F)
			{
				yield return Report.ConfigError(typeof(LocomotionMovementExtension),
					"must have a moveSpeedMultiplier multiplier different than 1.");
			}
		}
	}
}