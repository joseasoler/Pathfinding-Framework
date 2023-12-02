using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Prevent aquatic animals from being chosen as starting pets.
	/// </summary>
	[HarmonyPatch(typeof(ScenPart_StartingAnimal), "RandomPets")]
	public static class ScenPart_StartingAnimal_RandomPets_Patch
	{
		public static IEnumerable<PawnKindDef> Postfix(IEnumerable<PawnKindDef> __result)
		{
			foreach (PawnKindDef pawnKindDef in __result)
			{
				MovementDef movementDef = pawnKindDef.race?.MovementDef();
				if (movementDef == null || movementDef.defaultCost.cost < PathCost.Avoid.cost)
				{
					yield return pawnKindDef;
				}
			}
		}
	}
}