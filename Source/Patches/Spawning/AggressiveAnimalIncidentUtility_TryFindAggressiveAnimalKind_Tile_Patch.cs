using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	[HarmonyPatch(typeof(AggressiveAnimalIncidentUtility),
		nameof(AggressiveAnimalIncidentUtility.TryFindAggressiveAnimalKind), new[]
		{
			typeof(float), typeof(int),
			typeof(PawnKindDef)
		}, new[]
		{
			ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out
		})]
	public static class AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Tile_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo removeWaterAnimalsIfNeeded =
				AccessTools.Method(typeof(AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Utility),
					nameof(AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Utility.RemoveWaterAnimalsIfNeeded));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stloc_1) // Stores the list of valid manhunter PawnKindDefs
				{
					// Filter the list again if needed.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // tile
					yield return new CodeInstruction(OpCodes.Call, removeWaterAnimalsIfNeeded);
				}

				yield return instruction;
			}
		}
	}
}