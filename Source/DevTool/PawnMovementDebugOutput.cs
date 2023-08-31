using System;
using System.Collections.Generic;
using PathfindingFramework.Cache;
using PathfindingFramework.Cache.Global;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Generate a report with the movement type used by each pawn on the current map.
	/// </summary>
	public static class PawnMovementDebugOutput
	{
		[DebugOutput(category: PathfindingFramework.Name, onlyWhenPlaying: true)]
		public static void PawnLocomotion()
		{
			var input = new List<Tuple<string, string, string, string, string>>();
			foreach (var map in Find.Maps)
			{
				foreach (var pawn in map.mapPawns.AllPawnsSpawned)
				{
					string name = pawn.Name != null ? pawn.Name.ToString() : pawn.def.label;
					string thingID = pawn.ThingID;
					string mapId = map.ToString();
					string movementDefName = pawn.MovementDef().ToString();
					bool shouldAvoidFences = pawn.ShouldAvoidFences;
					input.Add(new Tuple<string, string, string, string, string>(name, thingID, mapId, movementDefName,
						shouldAvoidFences.ToString()));
				}
			}

			var dataTable = new string[5, input.Count + 1];
			dataTable[0, 0] = "Name";
			dataTable[1, 0] = "Id";
			dataTable[2, 0] = "Map";
			dataTable[3, 0] = "Movement";
			dataTable[4, 0] = "Avoid fences";

			for (var inputIndex = 0; inputIndex < input.Count; ++inputIndex)
			{
				var tuple = input[inputIndex];
				dataTable[0, inputIndex + 1] = tuple.Item1;
				dataTable[1, inputIndex + 1] = tuple.Item2;
				dataTable[2, inputIndex + 1] = tuple.Item3;
				dataTable[3, inputIndex + 1] = tuple.Item4;
				dataTable[4, inputIndex + 1] = tuple.Item5;
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}