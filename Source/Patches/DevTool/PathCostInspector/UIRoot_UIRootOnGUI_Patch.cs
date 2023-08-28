using HarmonyLib;
using PathfindingFramework.DevTool;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathCostInspector
{
	/// <summary>
	/// Show the path cost inspector window if it is enabled.
	/// </summary>
	[HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]
	internal static class UIRoot_UIRootOnGUI_Patch
	{
		internal static void Postfix()
		{
			PathCostInspectorDrawer.OnGui();
		}
	}
}