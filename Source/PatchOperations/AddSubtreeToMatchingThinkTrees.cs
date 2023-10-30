using System.Collections.Generic;
using System.Text;
using System.Xml;
using Verse;
using Verse.AI;

namespace PathfindingFramework.PatchOperations
{
	/// <summary>
	/// Iterate over all think trees. Look for a specific subtree. If the tree has it, add another subtree right before.
	/// </summary>
	public class AddSubtreeToMatchingThinkTrees : PatchOperation
	{
		/// <summary>
		/// Subtree to be added.
		/// </summary>
		public string subtree;

		/// <summary>
		/// The subtree will be added to all think trees having this think tree. It will be added before it.
		/// </summary>
		public string addBeforeSubtree;

		private const string ClassAttributeName = "Class";
		private const string TreeDefNodeName = "treeDef";

		private XmlNode FindChild(XmlNode parentNode, string childName)
		{
			if (parentNode == null)
			{
				return null;
			}

			foreach (XmlNode childNode in parentNode.ChildNodes)
			{
				if (childNode.Name == childName)
				{
					return childNode;
				}
			}

			return null;
		}

		private XmlNode FindSubtree(XmlNode parentNode, string treeDef)
		{
			foreach (XmlNode childNode in parentNode.ChildNodes)
			{
				// The child must have a Class="ThinkNode_Subtree" attribute...
				XmlNode attributeNode = childNode.Attributes?.GetNamedItem(ClassAttributeName);
				if (attributeNode?.Value != nameof(ThinkNode_Subtree))
				{
					continue;
				}

				// ... and a grandchild with an inner text of treeDef.
				XmlNode treeDefNode = FindChild(childNode, TreeDefNodeName);
				if (treeDefNode != null && treeDefNode.InnerText == treeDef)
				{
					return childNode;
				}
			}

			return null;
		}

		protected override bool ApplyWorker(XmlDocument xml)
		{
			foreach (XmlNode thinkTreeDefNode in xml.FirstChild.ChildNodes)
			{
				if (thinkTreeDefNode.Name != nameof(ThinkTreeDef))
				{
					continue;
				}

				XmlNode thinkRootNode = FindChild(thinkTreeDefNode, nameof(ThinkTreeDef.thinkRoot));

				if (thinkRootNode == null)
				{
					continue;
				}

				XmlNode subNodesNode = FindChild(thinkRootNode, nameof(ThinkNode.subNodes));
				if (subNodesNode == null)
				{
					continue;
				}

				XmlNode subTreeNode = FindSubtree(subNodesNode, addBeforeSubtree);
				if (subTreeNode == null)
				{
					continue;
				}

				XmlElement treeDefNode = xml.CreateElement(TreeDefNodeName);
				treeDefNode.InnerText = subtree;

				XmlElement newSubTreeNode = xml.CreateElement("li");
				newSubTreeNode.SetAttribute(ClassAttributeName, nameof(ThinkNode_Subtree));
				newSubTreeNode.AppendChild(treeDefNode);
				subNodesNode.InsertBefore(newSubTreeNode, subTreeNode);
			}

			return true;
		}
	}
}