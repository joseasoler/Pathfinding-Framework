using System.Collections.Generic;
using System.Xml;
using Verse;

namespace PathfindingFramework.PatchOperations
{
	/// <summary>
	/// Adds a terrain tag to all specified TerrainDefs.
	/// Done in a single XML pass, with each TerrainDef being processed only once.
	/// If any of the TerrainDefs lacked a tags list, one will be created.
	/// Fails if any of the TerrainDefs is not present.
	/// </summary>
	public class AddTagsToTerrainDefs : PatchOperation
	{
		public struct TagOperation
		{
			public string tag;
			public List<string> terrainDefs;
		}

		public List<List<TagOperation>> operations;

		private const string DefNameIdentifier = "defName";
		private const string TagsIdentifier = "tags";

		private static readonly HashSet<string> TerrainDefNames = new()
		{
			// RimWorld
			"TerrainDef",
			// Biomes! Core
			"BiomesCore.ActiveTerrainDef",
			// Vanilla Expanded Framework
			"VFECore.ActiveTerrainDef"
		};

		/// <summary>
		/// Translate the provided tag groups into a format more performant for XML traversal.
		/// </summary>
		/// <returns>Dictionary of terrain defs, each containing a list of tags.</returns>
		private Dictionary<string, HashSet<string>> GetTagsByTerrainDef()
		{
			Dictionary<string, HashSet<string>> result = new Dictionary<string, HashSet<string>>();
			for (int tagGroupsIndex = 0; tagGroupsIndex < operations.Count; ++tagGroupsIndex)
			{
				List<TagOperation> currentOperationGroup = operations[tagGroupsIndex];
				for (int tagGroupIndex = 0; tagGroupIndex < currentOperationGroup.Count; ++tagGroupIndex)
				{
					TagOperation currentOperation = currentOperationGroup[tagGroupIndex];
					string currentTag = currentOperation.tag;

					for (int terrainDefIndex = 0; terrainDefIndex < currentOperation.terrainDefs.Count; ++terrainDefIndex)
					{
						string currentTerrainDef = currentOperation.terrainDefs[terrainDefIndex];
						if (!result.ContainsKey(currentTerrainDef))
						{
							result[currentTerrainDef] = new HashSet<string>();
						}

						result[currentTerrainDef].Add(currentTag);
					}
				}
			}

			return result;
		}

		protected override bool ApplyWorker(XmlDocument xml)
		{
			// Group of terrainDefs which need to be processed.
			Dictionary<string, HashSet<string>> remainingTerrainDefs = GetTagsByTerrainDef();

			foreach (XmlNode terrainDefNode in xml.FirstChild.ChildNodes)
			{
				if (!TerrainDefNames.Contains(terrainDefNode.Name))
				{
					// Process only TerrainDefs, all other nodes can be safely ignored.
					continue;
				}

				XmlNode tagsNode = null;
				string defNameValue = null;
				foreach (XmlNode terrainDefChild in terrainDefNode.ChildNodes)
				{
					if (terrainDefChild.Name == DefNameIdentifier)
					{
						if (remainingTerrainDefs.ContainsKey(terrainDefChild.InnerText))
						{
							defNameValue = terrainDefChild.InnerText;
						}
						else
						{
							// This TerrainDef does not require patching. Stop analyzing it.
							break;
						}
					}
					else if (terrainDefChild.Name == TagsIdentifier)
					{
						// The node already has a tags list.
						tagsNode = terrainDefChild;
					}

					if (tagsNode != null && defNameValue != null)
					{
						// All required nodes have been found. Stop the search.
						break;
					}
				}

				if (defNameValue != null)
				{
					// Add a tags list if the TerrainDef node lacks one.
					if (tagsNode == null)
					{
						XmlElement emptyTagsNode = xml.CreateElement(TagsIdentifier);
						tagsNode = terrainDefNode.AppendChild(emptyTagsNode);
					}

					// Add all required tags to the tags list.
					foreach (string tag in remainingTerrainDefs[defNameValue])
					{
						XmlElement tagNode = xml.CreateElement("li");
						tagNode.InnerText = tag;
						tagsNode.AppendChild(tagNode);
					}

					// Remove the TerrainDef from the set of remaining terrain defs to process.
					remainingTerrainDefs.Remove(defNameValue);
				}

				if (remainingTerrainDefs.Count == 0)
				{
					break;
				}
			}

			if (remainingTerrainDefs.Count > 0)
			{
				Report.Error(
					$"AddTagsToTerrainDefs could not find the following TerrainDefs: {string.Join(", ", remainingTerrainDefs.Keys)}");
			}

			return remainingTerrainDefs.Count == 0;
		}
	}
}