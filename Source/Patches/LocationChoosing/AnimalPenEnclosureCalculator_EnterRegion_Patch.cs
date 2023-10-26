using HarmonyLib;
using Verse;

namespace PathfindingFramework.Patches.LocationChoosing
{
	/// <summary>
	/// Consider regions with vanilla impassable terrains as enclosed.
	/// </summary>
	[HarmonyPatch(typeof(AnimalPenEnclosureCalculator), "EnterRegion")]
	internal static class AnimalPenEnclosureCalculator_EnterRegion_Patch
	{
		private static void Postfix(ref bool __result, Region from, Region to)
		{
			__result &= to.UniqueTerrainDef() == null;
		}
	}
}