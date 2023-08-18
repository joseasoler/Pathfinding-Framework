using System;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Def
{
	/// <summary>
	/// Handles Pathfinding Framework custom parsing code.
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
				ParseHelper.Parsers<PathCost>.Register(ParsePathCost);
				Report.Debug("Parser initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Parser initialization failed:");
				Report.Error($"{exception.ToString()}");
			}
		}

		/// <summary>
		/// Used to parse PathCost values found in XML files.
		/// </summary>
		/// <param name="value">string representation of a PathCost instance.</param>
		/// <returns></returns>
		private static PathCost ParsePathCost(string value)
		{
			return new PathCost(value);
		}
	}
}