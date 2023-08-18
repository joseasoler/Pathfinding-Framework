using System;
using System.Reflection;
using PathfindingFramework.Mod;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Log and error reporting utilities.
	/// </summary>
	public static class Report
	{
		private static readonly Assembly Reference = typeof(Report).Assembly;
		public static readonly string Name = Reference.GetName().Name;
		private static readonly Version Version = Reference.GetName().Version;
		private static readonly string Prefix = $"[{Name} v{Version}] ";

		/// <summary>
		/// Adds the mod name and version to every log message.
		/// </summary>
		/// <param name="original">Text to be prefixed.</param>
		/// <returns>Text with the prefix.</returns>
		private static string AddPrefix(string original)
		{
			return $"{Prefix}{original}";
		}

		/// <summary>
		/// Shows a line in the log. Usually used only by certain mod options.
		/// </summary>
		/// <param name="message">Message to log.</param>
		public static void Notice(string message)
		{
			Log.Message(AddPrefix(message));
		}

		/// <summary>
		/// Debug messages are usually disabled, unless enabled explicitly in the mod settings.
		/// </summary>
		/// <param name="message">Message to log.</param>
		public static void Debug(string message)
		{
			if (Settings.Values.DebugLog)
			{
				Notice(message);
			}
		}

		/// <summary>
		/// Errors are always logged regardless of mod settings.
		/// </summary>
		/// <param name="message">Message to log.</param>
		public static void Error(string message)
		{
			Log.Error(AddPrefix(message));
		}

		public static string ConfigError(Verse.Def def, string error)
		{
			return $"{Prefix} [{def.defName}]: {error}";
		}
	}
}