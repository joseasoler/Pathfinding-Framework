using System.Text;
using Verse;

namespace PathfindingFramework.ErrorHandling
{
	/// <summary>
	/// Generate a region / region link error report.
	/// </summary>
	public static class RegionErrorReport
	{
		private static bool AnyValidCell(Region region)
		{
			Map map = region.Map;
			CellIndices cellIndices = map.cellIndices;
			Region[] directGrid = map.regionGrid.DirectGrid;
			foreach (IntVec3 c in region.extentsClose)
			{
				if (region.Equals(directGrid[cellIndices.CellToIndex(c)]))
				{
					return true;
				}
			}

			return false;
		}

		public static bool TryGet(Map map, out string report)
		{
			bool errors = false;
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"Region error report for map {map.GetUniqueLoadID()}");
			sb.AppendLine("---");
			foreach (Region region in map.regionGrid.AllRegions_NoRebuild_InvalidAllowed)
			{
				if (region == null)
				{
					sb.AppendLine("Null region");
					errors = true;
				}

				if (!AnyValidCell(region))
				{
					sb.AppendLine($"Region lacking cells: {region.ToReportString()}");
					errors = true;
				}

				foreach (RegionLink link in region.links)
				{
					if (link.RegionA == null || link.RegionB == null)
					{
						errors = true;
						sb.AppendLine($"Region {region.id.ToString()} has a link with errors: {link.ToReportString()}");
					}
				}
			}

			report = errors ? sb.ToString() : "";
			return errors;
		}
	}
}