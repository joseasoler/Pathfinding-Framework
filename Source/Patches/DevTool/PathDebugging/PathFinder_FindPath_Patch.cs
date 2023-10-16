namespace PathfindingFramework.Patches.DevTool.PathDebugging
{
	/*
	/// <summary>
	/// Allow debugging PathFinder.FindPath failures. Commented out by default for performance reasons.
	/// ToDo: better way to conditionally enable this feature.
	/// </summary>
	[HarmonyPatch(typeof(PathFinder), nameof(PathFinder.FindPath), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(TraverseParms), typeof(PathEndMode), typeof(PathFinderCostTuning))]
	internal static class PathFinder_FindPath_Patch
	{
		private static void AddData(PathFinder instance, string str)
		{
			PathFinderErrorDebug.AddPathFinderData(instance.map, instance.openList, instance.pathGrid, str);
		}

		internal static void Prefix(IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParms)
		{
			PathFinderErrorDebug.StartEntry(start, dest, traverseParms);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo startProfilingMethod = AccessTools.Method(typeof(PathFinder), "PfProfilerBeginSample");
			MethodInfo addDataMethod =
				AccessTools.Method(typeof(PathFinder_FindPath_Patch), nameof(AddData));

			MethodInfo logErrorMethod = AccessTools.Method(typeof(Log), nameof(Log.Error), new[] {typeof(string)});
			MethodInfo logWarningMethod = AccessTools.Method(typeof(Log), nameof(Log.Warning), new[] {typeof(string)});
			MethodInfo enablePrintMethod =
				AccessTools.Method(typeof(PathFinderErrorDebug), nameof(PathFinderErrorDebug.EnablePrint));

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

		internal static void Postfix()
		{
			PathFinderErrorDebug.FinishEntry();
		}
	}
	*/
}