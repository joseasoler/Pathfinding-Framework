using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Cache.Global;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Remove animals with MovementDef.manhuntersRequireWater enabled from maps lacking access to water.
	/// </summary>
	[HarmonyPatch(typeof(ManhunterPackIncidentUtility), nameof(ManhunterPackIncidentUtility.TryFindManhunterAnimalKind))]
	public class ManhunterPackIncidentUtility_TryFindManhunterAnimalKind_Patch
	{
		private static bool HasAccessToWater(int tileID)
		{
			WorldGrid grid = Find.WorldGrid;
			if (!grid.InBounds(tileID))
			{
				return false;
			}

			List<int> outNeighbors = new List<int>();
			grid.GetTileNeighbors(tileID, outNeighbors);
			foreach (int currentTileID in outNeighbors)
			{
				if (grid[currentTileID].biome == BiomeDefOf.Ocean)
				{
					return true;
				}
			}

			return false;
		}

		private static IEnumerable<PawnKindDef> RemoveWaterManhuntersIfNeeded(IEnumerable<PawnKindDef> allManhunters,
			int tileID)
		{
			bool accessToWater = HasAccessToWater(tileID);
			foreach (PawnKindDef manhunterDef in allManhunters)
			{
				MovementDef movementDef = MovementExtensionCache.GetMovementDef(manhunterDef);
				if (accessToWater || movementDef == null || !movementDef.manhuntersRequireWater)
				{
					yield return manhunterDef;
				}
			}
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo removeWaterManhuntersIfNeededMethod =
				AccessTools.Method(typeof(ManhunterPackIncidentUtility_TryFindManhunterAnimalKind_Patch),
					nameof(RemoveWaterManhuntersIfNeeded));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stloc_1) // Stores the list of valid manhunter PawnKindDefs
				{
					// Filter the list again if needed.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // tile
					yield return new CodeInstruction(OpCodes.Call, removeWaterManhuntersIfNeededMethod);
				}

				yield return instruction;
			}
		}
	}
}