using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Log and error reporting utilities.
	/// </summary>
	public static class Report
	{
		/// <summary>
		/// Assembly of the mod.
		/// </summary>
		private static readonly Assembly Reference = typeof(Report).Assembly;

		/// <summary>
		/// Current version of the assembly.
		/// </summary>
		private static readonly Version Version = Reference.GetName().Version;

		/// <summary>
		/// Prefix used in configuration errors and logs.
		/// </summary>
		private static readonly string Prefix = $"[{PathfindingFramework.Name} v{Version}] ";

		private static readonly HashSet<int> UsedHashes = new HashSet<int>();

		/// <summary>
		/// Prepends the identification prefix to some text to be used in a report.
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

		/// <summary>
		/// Display an error only if it has not be shown before.
		/// </summary>
		/// <param name="message">Message to log.</param>
		public static void ErrorOnce(string message)
		{
			var hash = message.GetHashCode();
			if (UsedHashes.Contains(hash))
			{
				return;
			}

			UsedHashes.Add(hash);
			Report.Error(message);
		}

		/// <summary>
		/// Generate a configuration error string to be yielded in a Def.ConfigErrors call.
		/// </summary>
		/// <param name="def">Def with a configuration error.</param>
		/// <param name="error">Error to show.</param>
		/// <returns>Configuration error string.</returns>
		public static string ConfigError(Def def, string error)
		{
			return $"{Prefix} [{def.defName}]: {error}";
		}

		/// <summary>
		/// Generate a configuration error string to be yielded in a ConfigErrors call not associated with a def.
		/// </summary>
		/// <param name="type">Type with a configuration error.</param>
		/// <param name="error">Error to show.</param>
		/// <returns>Configuration error string.</returns>
		public static string ConfigError(Type type, string error)
		{
			return $"{Prefix} [{type.Name}]: {error}";
		}
	}
}