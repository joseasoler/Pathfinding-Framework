using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Responsible for overriding the default movement types of pawn races using the values from the settings.
	/// </summary>
	public static class PawnMovementOverrideSettings
	{
		private static Dictionary<ThingDef, MovementDef> _originalMovementDefs = new();
		private static Dictionary<string, MovementDef> _movementDefsByDefName;

		/// <summary>
		/// Assumes that GetSettings and MovementExtensionReader.Initialize have been called.
		/// </summary>
		public static void Initialize()
		{
			_movementDefsByDefName = new Dictionary<string, MovementDef>();

			foreach (MovementDef movementDef in DefDatabase<MovementDef>.AllDefsListForReading)
			{
				_movementDefsByDefName[movementDef.defName] = movementDef;
			}

			foreach (ThingDef raceDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (Settings.Values.PawnMovementOverrides.TryGetValue(raceDef.defName, out string movementDefName) &&
				    _movementDefsByDefName.TryGetValue(movementDefName, out MovementDef movementDef))
				{
					raceDef.MovementDef() = movementDef;
				}
			}
		}

		public static void AddOriginal(ThingDef thingDef, MovementDef movementDef)
		{
			_originalMovementDefs[thingDef] = movementDef;
		}

		/// <summary>
		/// Movement type currently selected for the provided race.
		/// </summary>
		/// <param name="raceDef"></param>
		/// <returns></returns>
		public static MovementDef CurrentMovementDef(ThingDef raceDef)
		{
			if (Settings.Values.PawnMovementOverrides.TryGetValue(raceDef.defName, out string movementDefName) &&
			    _movementDefsByDefName.TryGetValue(movementDefName, out MovementDef cachedMovementDef))
			{
				return cachedMovementDef;
			}

			return _originalMovementDefs.TryGetValue(raceDef, out MovementDef originalMovementDef)
				? originalMovementDef
				: MovementDefOf.PF_Movement_Terrestrial;
		}
	}
}