using System;
using Verse;
using Verse.AI;

namespace PathfindingFramework.ErrorHandling
{
	/// <summary>
	/// Utility functions for error report generation.
	/// </summary>
	public static class ToReportStringUtil
	{
		public static string ToReportString(this IntVec3 cell)
		{
			return $"Cell[{cell.x}, {cell.z}]";
		}

		public static string ToReportString(this IntVec3 cell, Map map)
		{
			string terrainDef = map != null && cell.InBounds(map) ? cell.GetTerrain(map).defName : "Unknown";
			return $"Cell[{cell.x}, {cell.z}, {terrainDef}]";
		}
		
		public static string ToReportString(this TraverseMode mode)
		{
			return  Enum.GetName(typeof(TraverseMode), mode);
		}


		public static string ToReportString(this PathEndMode mode)
		{
			return  Enum.GetName(typeof(PathEndMode), mode);
		}

		public static string ToReportString(this Region region)
		{
			if (region == null)
			{
				return "Region[id: Null]";
			}

			string doorStr = region.door == null ? "Null" : region.door.GetUniqueLoadID();
			return
				$"Region[id:{region.id}, map:{region.Map.GetUniqueLoadID()}] -> extents:{region.extentsClose}, links:{region.links.Count}, cells:{region.CellCount}door:{doorStr}";
		}

		public static string ToReportString(this RegionLink link)
		{
			string regionA = link.RegionA != null ? link.RegionA.id.ToString() : "Null";
			string regionB = link.RegionB != null ? link.RegionB.id.ToString() : "Null";

			return
				$"RegionLink[regionA:{regionA}, regionB:{regionB}] -> root:[{link.span.root.x}, {link.span.root.z}], dir:{Enum.GetName(typeof(SpanDirection), link.span.dir)}, len:{link.span.length.ToString()}";
		}
	}
}