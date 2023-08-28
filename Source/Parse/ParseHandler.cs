using System;
using System.Globalization;
using Verse;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Handles the initialization of custom parsing code.
	/// </summary>
	public class ParseHandler
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
		public static short ParseShort(string str)
		{
			short result;
			if (!short.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				Report.Error($"Could not parse short value {str}");
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