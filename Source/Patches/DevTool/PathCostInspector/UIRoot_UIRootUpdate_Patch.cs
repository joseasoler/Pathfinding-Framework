using HarmonyLib;
using PathfindingFramework.DevTool;
using Verse;

namespace PathfindingFramework.Patches.DevTool.PathCostInspector
{
	/// <summary>
	/// Enable or disable the path cost inspector window depending on mod settings and current game state.
	/// </summary>
	[HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootUpdate))]
	internal static class UIRoot_UIRootUpdate_Patch
	{
		internal static void Postfix()
		{
			PathCostInspectorDrawer.Update();
		}
	}
}