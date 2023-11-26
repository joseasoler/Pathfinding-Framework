using HarmonyLib;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Prevent aquatic animals from being chosen as starting pets.
	/// </summary>
	[HarmonyPatch(typeof(ScenPart_StartingAnimal), "PetWeight")]
	public static class ScenPart_StartingAnimal_PetWeight_Patch
	{
		public static void Postfix(float __result, PawnKindDef animal)
		{
			MovementDef movementDef = animal.race?.MovementDef();
			if (movementDef != null && movementDef.defaultCost.cost >= PathCost.Avoid.cost)
			{
				__result = 0.0F;
			}
		}
	}
}