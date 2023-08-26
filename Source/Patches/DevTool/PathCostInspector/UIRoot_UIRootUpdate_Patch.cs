using HarmonyLib;
using PathfindingFramework.Debug;
using Verse;

namespace PathfindingFramework.Patches.Debug.PathCostInspector
{
	/// <summary>
	/// Enable or disable the path cost inspector window depending on mod settings and current game state.
	/// </summary>
	[HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootUpdate))]
	public class UIRoot_UIRootUpdate_Patch
	{
		internal static void Postfix()
		{
			PathCostInspectorDrawer.Update();
		}
	}
}