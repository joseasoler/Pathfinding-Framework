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
			typeof(float), typeof(Map),
			typeof(PawnKindDef)
		}, new[]
		{
			ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out
		})]
	public static class AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Map_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo removeWaterAnimalsIfNeededMethod =
				AccessTools.Method(typeof(AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Utility),
					nameof(AggressiveAnimalIncidentUtility_TryFindAggressiveAnimalKind_Utility.RemoveWaterAnimalsIfNeeded));

			MethodInfo getMapTileMethod = AccessTools.PropertyGetter(typeof(Map), nameof(Map.Tile));
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stloc_1) // Stores the list of valid manhunter PawnKindDefs
				{
					// Filter the list again if needed.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // map
					yield return new CodeInstruction(OpCodes.Callvirt, getMapTileMethod); // tile
					yield return new CodeInstruction(OpCodes.Call, removeWaterAnimalsIfNeededMethod);
				}

				yield return instruction;
			}
		}
	}
}