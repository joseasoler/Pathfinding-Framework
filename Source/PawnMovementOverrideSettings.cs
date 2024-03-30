using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Responsible for overriding the default movement types of pawn races using the values from the settings.
	/// </summary>
	public static class PawnMovementOverrideSettings
	{
		private static Dictionary<string, MovementDef> _movementDefsByDefName;

		/// <summary>
		/// Assumes that GetSettings and MovementExtensionCache.Initialize have been called.
		/// </summary>
		public static void Initialize()
		{
			_movementDefsByDefName = new Dictionary<string, MovementDef>();

			foreach (MovementDef movementDef in DefDatabase<MovementDef>.AllDefsListForReading)
			{
				_movementDefsByDefName[movementDef.defName] = movementDef;
			}
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

			return MovementDefDatabase<ThingDef>.Get(raceDef);
		}
	}
}