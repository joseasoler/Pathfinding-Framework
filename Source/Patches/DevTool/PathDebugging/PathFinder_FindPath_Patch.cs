using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PathfindingFramework.ErrorHandling;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/*
	/// <summary>
	/// Allow debugging PathFinder.FindPath failures. Commented out by default for performance reasons.
	/// Failures of this kind have not been common through development, so there was no need to add an option for this.
	/// </summary>
	[HarmonyPatch(typeof(PathFinder), nameof(PathFinder.FindPath), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(TraverseParms), typeof(PathEndMode), typeof(PathFinderCostTuning))]
	public static class PathFinder_FindPath_Patch
	{
		public static void AddData(PathFinder instance, string str)
		{
			PathFinderErrorReport.AddPathFinderData(instance.map, instance.openList, instance.pathGrid, str);
		}

		public static void Prefix(IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParms)
		{
			PathFinderErrorReport.StartEntry(start, dest, traverseParms);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo startProfilingMethod = AccessTools.Method(typeof(PathFinder), "PfProfilerBeginSample");
			MethodInfo addDataMethod =
				AccessTools.Method(typeof(PathFinder_FindPath_Patch), nameof(AddData));

			MethodInfo logErrorMethod = AccessTools.Method(typeof(Log), nameof(Log.Error), new[] {typeof(string)});
			MethodInfo logWarningMethod = AccessTools.Method(typeof(Log), nameof(Log.Warning), new[] {typeof(string)});
			MethodInfo enablePrintMethod =
				AccessTools.Method(typeof(PathFinderErrorReport), nameof(PathFinderErrorReport.EnablePrint));

			foreach (CodeInstruction instruction in instructions)
			{
				MethodInfo methodOperand = instruction.operand as MethodInfo;
				if (methodOperand == startProfilingMethod)
				{
					yield return new CodeInstruction(OpCodes.Call, addDataMethod);
				}
				else if (methodOperand == logErrorMethod || methodOperand == logWarningMethod)
				{
					yield return new CodeInstruction(OpCodes.Call, enablePrintMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}

		public static void Postfix()
		{
			PathFinderErrorReport.FinishEntry();
		}
	}
	*/
}