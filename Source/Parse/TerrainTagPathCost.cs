using System.Xml;
using Verse;

namespace PathfindingFramework.Def
{
	/// <summary>
	/// Maps a terrain tag to a pathing cost.
	/// </summary>
	public class TerrainTagPathCost
	{
		public string tag;
		public PathCost cost;

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