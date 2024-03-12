using System;
using System.Globalization;
using Verse;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Handles the initialization of custom parsing code.
	/// </summary>
	public static class ParseHandler
	{
		/// <summary>
		/// Initialize the custom parsing code.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				if (!ParseHelper.parsers.ContainsKey(typeof(short)))
				{
					ParseHelper.Parsers<short>.Register(ParseShort);
				}

				ParseHelper.Parsers<PathCost>.Register(ParsePathCost);
				Report.Debug("Parser initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Parser initialization failed:");
				Report.Error($"{exception}");
			}
		}

		/// <summary>
		/// See ParseHelper.ParseIntPermissive.
		/// </summary>
		/// <param name="value">string representation of a short value.</param>
		/// <returns>short value</returns>
		private static short ParseShort(string value)
		{
			if (!short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out short result))
			{
				Report.Error($"Could not parse short value {value}");
			}

			return result;
		}

		/// <summary>
		/// Used to parse PathCost values found in XML files.
		/// </summary>
		/// <param name="value">string representation of a PathCost instance.</param>
		/// <returns>PathCost instance</returns>
		private static PathCost ParsePathCost(string value)
		{
			return new PathCost(value);
		}
	}
}