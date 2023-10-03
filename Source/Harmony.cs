using System;
using System.IO;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace PathfindingFramework
{
	/// <summary>
	/// Handles the application of harmony patches.
	/// </summary>
	public static class Harmony
	{
		/// <summary>
		/// Apply harmony patches to the game.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				var path = Path.Combine(Path.GetTempPath(), "PF_Harmony.txt");
				Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", path);
				// HarmonyLib.Harmony.DEBUG = true;
				var harmonyInstance = new HarmonyLib.Harmony(PathfindingFramework.PackageId);
				harmonyInstance.PatchAll();
				Report.Debug("Harmony patching applied.");
			}
			catch (Exception exception)
			{
				Report.Error("Harmony patching failed:");
				Report.Error($"{exception}");
			}
		}
	}
}