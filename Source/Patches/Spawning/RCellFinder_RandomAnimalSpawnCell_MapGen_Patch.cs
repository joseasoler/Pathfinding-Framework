using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

/*
namespace PathfindingFramework.Patches.Spawning
{
	/// <summary>
	/// Allow animal spawns to take place over terrain types that do not allow wandering.
	/// </summary>
	[HarmonyPatch]
	internal static class RCellFinder_RandomAnimalSpawnCell_MapGen_Patch
	{
		/// <summary>
		/// Delegate used internally by RCellFinder.RandomAnimalSpawnCell_MapGen to find desirable tiles.
		/// </summary>
		/// <returns>Method information of the delegate to be patched.</returns>
		static MethodBase TargetMethod()
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			return typeof(RCellFinder).GetNestedTypes(flags)
				.SelectMany(nestedType => nestedType.GetMethods(flags))
				.FirstOrDefault(method => method.Name.Contains(nameof(RCellFinder.RandomAnimalSpawnCell_MapGen)));
		}

		/// <summary>
		/// Removes the if block that checks c.GetTerrain(map).avoidWander.
		/// </summary>
		/// <param name="instructions">Original code instructions.</param>
		/// <returns>Patched code instructions.</returns>
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var instructionList = instructions.ToList();

			// Index in which avoidWander appears.
			var avoidWander = AccessTools.Field(typeof(TerrainDef), nameof(TerrainDef.avoidWander));
			var avoidWanderIndex = instructionList.FindIndex(
				instruction => instruction.opcode == OpCodes.Ldfld
				               && (FieldInfo) instruction.operand == avoidWander);

			// Offsets to the beginning and end of the if block.
			var ignoreStart = avoidWanderIndex - 4;
			var ignoreEnd = avoidWanderIndex + 3;

			if (avoidWanderIndex == -1)
			{
				Report.Error(
					$"{nameof(RCellFinder_RandomAnimalSpawnCell_MapGen_Patch)} failed. Could not find avoidWander call in delegate!");
				ignoreStart = int.MaxValue;
				ignoreEnd = int.MinValue;
			}

			// The label for jumping from the avoidWander block to the next one is now unused.
			instructionList[ignoreEnd + 1].ExtractLabels();
			// Replace it with the label that was pointing to the avoidWander block.
			instructionList[ignoreStart].MoveLabelsTo(instructionList[ignoreEnd + 1]);
			// Generate the new instruction list.
			for (var index = 0; index < instructionList.Count; ++index)
			{
				if (index < ignoreStart || index > ignoreEnd)
				{
					yield return instructionList[index];
				}
			}
		}
	}
}
*/