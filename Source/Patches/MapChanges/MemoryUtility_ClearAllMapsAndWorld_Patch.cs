using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse.Profile;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// Clear the map path cost cache after going back to the main menu.
	/// </summary>
	[HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
	internal static class MemoryUtility_ClearAllMapsAndWorld_Patch
	{
		internal static void Postfix()
		{
			MapPathCostCache.Clear();
		}
	}
}