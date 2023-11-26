using System;
using System.IO;
using PathfindingFramework.Patches.ModCompatibility.GeologicalLandforms;

namespace PathfindingFramework
{
	/// <summary>
	/// Handles the application of harmony patches.
	/// </summary>
	public static class Harmony
	{
		private static HarmonyLib.Harmony harmonyInstance;

		/// <summary>
		/// Apply harmony patches to the game.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				string path = Path.Combine(Path.GetTempPath(), "PF_Harmony.txt");
				Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", path);
				// HarmonyLib.Harmony.DEBUG = true;
				harmonyInstance = new HarmonyLib.Harmony(PathfindingFrameworkMod.PackageId);
				harmonyInstance.PatchAll();
				Report.Debug("Harmony patching applied.");
			}
			catch (Exception exception)
			{
				Report.Error("Harmony patching failed:");
				Report.Error($"{exception}");
			}
		}

		public static void LoadingFinished()
		{
			GeologicalLandformsHarmony.ApplyPatches(harmonyInstance);
		}
	}
}