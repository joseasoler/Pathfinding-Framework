using System;
using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.DevTool
{
	/// <summary>
	/// Generate a report with the movement type used by each pawn on the current map.
	/// </summary>
	public static class PawnMovementDebugOutput
	{
		[DebugOutput(category: PathfindingFrameworkMod.Name, onlyWhenPlaying: true)]
		public static void CurrentPawnMovement()
		{
			var input = new List<Tuple<string, string, string, string, string, string>>();
			foreach (Map map in Find.Maps)
			{
				foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
				{
					string name = pawn.Name != null ? pawn.Name.ToString() : pawn.def.LabelCap;
					string thingID = pawn.ThingID.CapitalizeFirst();
					string mapId = map.ToString();
					string movementDefName = pawn.MovementDef().ToString();
					string shouldAvoidFences = pawn.MovementContext().ShouldAvoidFences.ToString().CapitalizeFirst();
					string ignoreFire = pawn.MovementContext().CanIgnoreFire.ToString().CapitalizeFirst();
					input.Add(new Tuple<string, string, string, string, string, string>(name, thingID, mapId, movementDefName,
						shouldAvoidFences, ignoreFire));
				}
			}

			string[,] dataTable = new string[6, input.Count + 1];
			dataTable[0, 0] = "Name";
			dataTable[1, 0] = "Id";
			dataTable[2, 0] = "Map";
			dataTable[3, 0] = "Movement";
			dataTable[4, 0] = Translations.PF_NoFencesMovementLabel;
			dataTable[5, 0] = Translations.PF_IgnoreFireMovementLabel;

			for (int inputIndex = 0; inputIndex < input.Count; ++inputIndex)
			{
				var tuple = input[inputIndex];
				dataTable[0, inputIndex + 1] = tuple.Item1;
				dataTable[1, inputIndex + 1] = tuple.Item2;
				dataTable[2, inputIndex + 1] = tuple.Item3;
				dataTable[3, inputIndex + 1] = tuple.Item4;
				dataTable[4, inputIndex + 1] = tuple.Item5;
				dataTable[5, inputIndex + 1] = tuple.Item6;
			}

			Find.WindowStack.Add(new Window_DebugTable(dataTable));
		}
	}
}