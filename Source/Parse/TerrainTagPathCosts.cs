using System.Collections.Generic;
using System.Xml;
using Verse;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Saner syntax for writing a terrain to path cost dictionary using XML.
	/// </summary>
	public class TerrainTagPathCosts
	{
		/// <summary>
		/// Maps terrain tags to their path costs in this movement type.
		/// </summary>
		public Dictionary<string, PathCost> data;

		/// <summary>
		/// Parse an instance of this class from XML.
		/// </summary>
		/// <param name="xmlRoot">XML node to parse.</param>
		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			data = new Dictionary<string, PathCost>();
			var tagCosts = DirectXmlToObject.ListFromXml<TerrainTagPathCost>(xmlRoot);
			foreach (var tagCost in tagCosts)
			{
				data.Add(tagCost.tag, tagCost.cost);
			}
		}
	}
}