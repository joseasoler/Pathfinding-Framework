using System;
using System.Collections.Generic;
using LudeonTK;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Display a report of every terrain tag and their associated terrains.
	/// </summary>
	public class TerrainTagsDebugOutput
	{
		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: false)]
		public static void TerrainTags()
		{
			var input = new List<Tuple<string, string, string>>();

			foreach (TerrainDef terrainDef in DefDatabase<TerrainDef>.AllDefsListForReading)
			{
				if (terrainDef.tags == null || terrainDef.generated)
				{
					// For brevity, generated terrainDefs such as carpets are excluded.
					continue;
				}

				string packageId;
				if (terrainDef.modContentPack == null)
				{
					packageId = "Unknown";
				}
				else
				{
					packageId = terrainDef.modContentPack.PackageIdPlayerFacing ?? "Unknown";
				}

				string terrainDefName = terrainDef.defName;
				foreach (string tag in terrainDef.tags)
				{
					input.Add(new Tuple<string, string, string>(tag, terrainDefName, packageId));
				}
			}

			input.Sort();

			string[,] dataTable = new string[3, input.Count + 1];
			dataTable[0, 0] = "Tag";
			dataTable[1, 0] = "TerrainDef";
			dataTable[2, 0] = "PackageId";

			for (int inputIndex = 0; inputIndex < input.Count; ++inputIndex)
			{
				var tuple = input[inputIndex];
				dataTable[0, inputIndex + 1] = tuple.Item1;
				dataTable[1, inputIndex + 1] = tuple.Item2;
				dataTable[2, inputIndex + 1] = tuple.Item3;
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}