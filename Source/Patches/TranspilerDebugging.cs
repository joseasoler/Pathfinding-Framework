using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace PathfindingFramework.Patches
{
	/// <summary>
	/// Utilities to help with the development of transpilers.
	/// </summary>
	public static class TranspilerDebugging
	{
		public static IEnumerable<CodeInstruction> Print(string label, IEnumerable<CodeInstruction> instructions)
		{
			StringBuilder sb = new StringBuilder();
			if (label != "")
			{
				sb.AppendLine($"---------- {label} ----------");
			}
			foreach (CodeInstruction instruction in instructions)
			{
				sb.AppendLine(instruction.ToString());
				yield return instruction;
			}

			Report.Error(sb.ToString());
		}
	}
}