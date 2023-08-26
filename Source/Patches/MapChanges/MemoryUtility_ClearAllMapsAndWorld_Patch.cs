using HarmonyLib;
using PathfindingFramework.Cache.Local;
using Verse;
using Verse.Profile;

namespace PathfindingFramework.Patches.MapChanges
{
	/// <summary>
	/// Maps do not notify their components of destruction when going back to the main menu.
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