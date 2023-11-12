using HarmonyLib;
using PathfindingFramework.DevTool;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathCostInspector
{
	/// <summary>
	/// Show the path cost inspector window if it is enabled.
	/// </summary>
	[HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]
	public static class UIRoot_UIRootOnGUI_Patch
	{
		public static void Postfix()
		{
			InspectorDrawer.OnGui();
		}
	}
}