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
		public static IEnumerable<CodeInstruction> Print(IEnumerable<CodeInstruction> instructions, string label = "")
		{
			StringBuilder sb = new StringBuilder();
			if (label != "")
			{
				sb.AppendLine($"---------- {label} ----------");
			}

			foreach (CodeInstruction instruction in instructions)
			{
				sb.Append(instruction);
				if (instruction.operand != null)
				{
					sb.AppendLine($" of type {instruction.operand.GetType()}");
				}
				else
				{
					sb.AppendLine();
				}

				yield return instruction;
			}

			Report.Error(sb.ToString());
		}
	}
}