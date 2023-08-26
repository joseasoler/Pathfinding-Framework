using HarmonyLib;
using PathfindingFramework.Debug;
using Verse;

namespace PathfindingFramework.Patches.Debug.PathCostInspector
{
	/// <summary>
	/// Show the path cost inspector window if it is enabled.
	/// </summary>
	[HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]
	public class UIRoot_UIRootOnGUI_Patch
	{
		internal static void Postfix()
		{
			PathCostInspectorDrawer.OnGui();
		}
	}
}