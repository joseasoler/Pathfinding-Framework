using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Make the manhunter incident check movement type aware.
	/// Without this patch, TryFindManhunterAnimalKind could return an aquatic animal and then TryFindRandomPawnEntryCell
	/// would fail.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_ManhunterPack), "CanFireNowSub")]
	public static class IncidentWorker_ManhunterPack_CanFireNowSub_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return IncidentWorker_ManhunterPack_Util.Transpile_TryFindRandomPawnEntryCell(OpCodes.Ldloc_1, instructions);
		}
	}
}