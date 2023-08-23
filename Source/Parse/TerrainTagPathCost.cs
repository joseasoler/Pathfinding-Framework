using System.Xml;
using Verse;

namespace PathfindingFramework.Def
{
	/// <summary>
	/// Maps a terrain tag to a pathing cost.
	/// </summary>
	public class TerrainTagPathCost
	{
		/// <summary>
		/// One of the values used in TerrainDef.tags.
		/// </summary>
		public string tag;
		
		/// <summary>
		/// Path cost associated to this tag.
		/// </summary>
		public PathCost cost;

		/// <summary>
		/// Parse an instance of this class from XML.
		/// </summary>
		/// <param name="xmlRoot">XML node to parse.</param>
		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			if (xmlRoot.ChildNodes.Count != 1)
			{
				Report.Error($"TerrainTagPathCost must only have one child: {xmlRoot.OuterXml}");
			}
			else
			{
				tag = xmlRoot.Name;
				cost = ParseHelper.FromString<PathCost>(xmlRoot.FirstChild.Value);
			}
		}
	}
}