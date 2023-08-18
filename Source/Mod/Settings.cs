using Verse;

namespace PathfindingFramework.Mod
{
	/// <summary>
	/// Contains data for all settings values. The default values of this object are the initial default settings for the mod.
	/// </summary>
	public class SettingValues
	{
		/// <summary>
		/// Write additional debug information to the game log.
		/// </summary>
		public bool DebugLog = false;

		/// <summary>
		/// Write a report of all loaded movement types to the game log after game load.
		/// </summary>
		public bool MovementTypesReport = false;
	}

	public class Settings : ModSettings
	{
		public static SettingValues Values = new SettingValues();

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
			Scribe_Values.Look(ref Values.DebugLog, "DebugLog");
			Scribe_Values.Look(ref Values.MovementTypesReport, "MovementTypesReport");
		}
	}
}