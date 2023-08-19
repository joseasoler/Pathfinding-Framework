using System.Collections.Generic;
using System.Xml;
using Verse;

namespace PathfindingFramework.Def
{
	/// <summary>
	/// Saner syntax for writing a terrain to path cost dictionary using XML.
	/// </summary>
	public class TerrainTagPathCosts
	{
		public Dictionary<string, PathCost> data;

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			data = new Dictionary<string, PathCost>();
			var tagCosts = DirectXmlToObject.ListFromXml<TerrainTagPathCost>(xmlRoot);
			foreach (TerrainTagPathCost tagCost in tagCosts)
			{
				data.Add(tagCost.tag, tagCost.cost);
			}
		}
	}
}