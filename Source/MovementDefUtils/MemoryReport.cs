using System.Collections.Generic;
using PathfindingFramework.DevTool;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Generate a memory usage report for the path costs of MovementDefs.
	/// </summary>
	public static class MemoryReport
	{
		public static List<MemoryUsageData> Get()
		{
			int count = DefDatabase<TerrainDef>.AllDefsListForReading.Count *
			            DefDatabase<MovementDef>.AllDefsListForReading.Count * sizeof(short);
			return new List<MemoryUsageData>
			{
				new("DefDatabase<MovementDef>", MemoryUsageData.Global, "Path costs",
					count)
			};
		}
	}
}