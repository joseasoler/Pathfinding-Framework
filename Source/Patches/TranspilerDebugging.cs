using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace PathfindingFramework.Patches
{
	public static class TranspilerDebugging
	{
		public static IEnumerable<CodeInstruction> Print(string label, IEnumerable<CodeInstruction> instructions)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"---------- {label} ----------");
			foreach (CodeInstruction instruction in instructions)
			{
				sb.AppendLine(instruction.ToString());
				yield return instruction;
			}

			Report.Error(sb.ToString());
		}
	}
}