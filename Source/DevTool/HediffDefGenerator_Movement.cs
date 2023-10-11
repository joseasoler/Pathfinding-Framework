﻿using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Generate implied HediffDefs intended for debugging purposes.
	/// </summary>
	public static class HediffDefGenerator_Movement
	{
		/// <summary>
		/// Create a HediffDef for each MovementDef. They can be used to arbitrarily assign movement types for testing.
		/// </summary>
		public static void GenerateMovementHediffDefs()
		{
			var movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			for (int index = 0; index < movementDefs.Count; ++index)
			{
				var movementDef = movementDefs[index];
				var hediffDef = new HediffDef
				{
					defName = $"{movementDef.defName}_DebugHediff",
					label = $"Add movement type {movementDef.label}",
					description = $"Debug hediff autogenerated by the {PathfindingFramework.Name} mod.",
					modExtensions = new List<DefModExtension>
					{
						new MovementExtension
						{
							movementDef = movementDef
						}
					}
				};

				DefGenerator.AddImpliedDef(hediffDef);
			}
		}
	}
}