﻿using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Report about loaded movement types.
	/// </summary>
	public static class LoadedDataReport
	{
		public static void Write()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"Mod initialized.");
			if (Settings.Values.DebugLog)
			{
				AdditionalDebugInformation(sb);
			}
			else
			{
				sb.AppendLine($"MovementDef count: {DefDatabase<MovementDef>.AllDefsListForReading.Count}");
			}

			Report.Notice(sb.ToString());
		}

		public class MovementDefData : IComparable<MovementDefData>
		{
			public readonly string packageId;
			public readonly string defName;

			public MovementDefData(string packageId, string defName)
			{
				this.packageId = packageId;
				this.defName = defName;
			}

			public int CompareTo(MovementDefData other)
			{
				int packageIdComparison = string.Compare(packageId, other.packageId, StringComparison.Ordinal);
				return packageIdComparison != 0
					? packageIdComparison
					: string.Compare(defName, other.defName, StringComparison.Ordinal);
			}
		}

		private static void AdditionalDebugInformation(StringBuilder sb)
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			List<MovementDefData> data = new List<MovementDefData>();
			for (int index = 0; index < movementDefs.Count; ++index)
			{
				MovementDef movementDef = movementDefs[index];
				string packageId = movementDef.modContentPack?.PackageIdPlayerFacing ?? "Unknown";
				data.Add(new MovementDefData(packageId, movementDef.defName));
			}

			data.Sort();
			sb.AppendLine("Loaded MovementDefs:");
			foreach (MovementDefData entry in data)
			{
				sb.AppendLine($"{entry.defName} ({entry.packageId})");
			}
		}
	}
}