using System.Collections.Generic;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Contains data for all settings values. The default values of this object are the initial default settings for the mod.
	/// </summary>
	public class SettingValues
	{
		// General tab.

		/// <summary>
		/// Pawns with a flammability of zero ignore the pathfinding costs of fire.
		/// </summary>
		public bool IgnoreFire /* = false*/;

		/// <summary>
		/// If this setting is enabled, animals on small walkable zones of the map will occasionally relocate to other zones
		/// of the map, usually choosing zones that are close by, and larger than the current one.
		/// </summary>
		public bool WildAnimalRelocation = true;

		// Pawn movement tab.

		/// <summary>
		/// Stores the movementDef override for each pawnKindDef.race ThingDef.
		/// These are stored as strings to avoid issues when the modlist of a user changes.
		/// Pawns or movement types that are no longer present are silently ignored, and will be deleted if the settings are
		/// saved.
		/// </summary>
		public Dictionary<string, string> PawnMovementOverrides = new();

		// Debugging tab.

		/// <summary>
		/// Enable the inspectors.
		/// </summary>
		public bool Inspectors /* = false*/;

		/// <summary>
		/// Display a log with additional information when the pathfinder is unable to find a path.
		/// Keep in mind that this log triggers in many valid cases; enabling it is only recommended for debugging.
		/// </summary>
		public bool LogPathNotFound /* = false*/;

		/// <summary>
		/// Write additional debug information to the game log.
		/// </summary>
		public bool DebugLog /* = false*/;
	}

	/// <summary>
	/// Allows the rest of the mod to access a SettingValues instance. Handles resetting, save and load.
	/// </summary>
	public class Settings : ModSettings
	{
		/// <summary>
		/// Single instance of the setting values of this mod. Uses static for performance reasons.
		/// </summary>
		public static SettingValues Values = new();

		/// <summary>
		/// Set all settings to their default values.
		/// </summary>
		public static void Reset()
		{
			Values = new SettingValues();
		}

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();

			// General tab.
			Scribe_Values.Look(ref Values.IgnoreFire, nameof(Values.IgnoreFire));
			Scribe_Values.Look(ref Values.WildAnimalRelocation, nameof(Values.WildAnimalRelocation), defaultValue: true);

			// Pawn movement tab.
			Scribe_Collections.Look(ref Values.PawnMovementOverrides, nameof(Values.PawnMovementOverrides), LookMode.Value,
				LookMode.Value);

			// Debugging tab.
			Scribe_Values.Look(ref Values.Inspectors, nameof(Values.Inspectors));
			Scribe_Values.Look(ref Values.LogPathNotFound, nameof(Values.LogPathNotFound));
			Scribe_Values.Look(ref Values.DebugLog, nameof(Values.DebugLog));
		}
	}
}