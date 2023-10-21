using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.Patches.LocationChoosing;
using RimWorld;
using Verse;

namespace PathfindingFramework.Patches.Reachable
{
	/// <summary>
	/// By default, this function uses a random pawn from the faction that owns the map to calculate reachability.
	/// This usually means a random colony pawn. If the colony has tamed an aquatic or flying animal, and that animal
	/// is chosen, then wanderers, enemies, and any other kind of spawning pawns can appear in truly weird places.
	/// To prevent this situation, the call to SpawnedPawnsInFaction is replaced to return an empty list.
	/// </summary>
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReachFactionBase))]
	internal static class Reachability_CanReachFactionBase_Patch
	{
		private static List<Pawn> IgnoreSpawnedPawns(MapPawns mapPawns, Faction faction)
		{
			return new List<Pawn>();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalMethod =
				AccessTools.Method(typeof(MapPawns), nameof(MapPawns.SpawnedPawnsInFaction));

			MethodInfo modifiedMethod =
				AccessTools.Method(typeof(Reachability_CanReachFactionBase_Patch),
					nameof(IgnoreSpawnedPawns));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(originalMethod))
				{
					yield return new CodeInstruction(OpCodes.Call, modifiedMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}