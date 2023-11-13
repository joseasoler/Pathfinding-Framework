using System.Collections.Generic;
using PathfindingFramework.Patches;
using UnityEngine;
using Verse;

namespace PathfindingFramework.GenSteps
{
	/// <summary>
	/// Movement type aware replacement for RimWorld.GenStep_Animals.
	/// </summary>
	public class PF_GenStep_Animals : GenStep
	{
		public override int SeedPart => 1298760307;

		public override void Generate(Map map, GenStepParams parms)
		{
			int iterationCount = 0;

			List<PawnKindDef> availableAnimals = new List<PawnKindDef>();

			foreach (PawnKindDef pawnKindDef in map.Biome.AllWildAnimals)
			{
				if (map.mapTemperature.SeasonAcceptableFor(pawnKindDef.race))
				{
					availableAnimals.Add(pawnKindDef);
				}
			}

			while (!map.wildAnimalSpawner.AnimalEcosystemFull && availableAnimals.Count > 0)
			{
				++iterationCount;
				if (iterationCount >= 10000)
				{
					Report.Error($"{typeof(PF_GenStep_Animals)} reached the maximum allowed number of iterations.");
					break;
				}

				// Choose an animal first.
				availableAnimals.TryRandomElementByWeight(
					pawnKindDef => map.wildAnimalSpawner.CommonalityOfAnimalNow(pawnKindDef), out PawnKindDef chosenAnimalDef);

				MovementDef movementDef = chosenAnimalDef.race?.MovementDef() ?? MovementDefOf.PF_Movement_Terrestrial;
				if (!CellFinderLoose.TryGetRandomCellWith(testCell =>
						    LocationFinding.CanStandAt(movementDef, map, testCell) &&
						    LocationFinding.CanReachMapEdge(movementDef, map, testCell),
					    map, 1000, out IntVec3 randomCell))
				{
					Report.Error(
						$"{typeof(PF_GenStep_Animals)} could not find a random cell to spawn animal with PawnKindDef {chosenAnimalDef} using MovementDef {movementDef}");
					// Remove the chosen animal type from the list to avoid further errors.
					availableAnimals.Remove(chosenAnimalDef);
					continue;
				}

				int randomInRange = chosenAnimalDef.wildGroupSize.RandomInRange;
				int radius = Mathf.CeilToInt(Mathf.Sqrt(chosenAnimalDef.wildGroupSize.max));

				// Generate the first pawn in the chosen tile.
				Pawn pawn = GenSpawn.Spawn(PawnGenerator.GeneratePawn(chosenAnimalDef), randomCell, map) as Pawn;
				for (int index = 1; index < randomInRange; ++index)
				{
					// Use the first pawn to calculate close valid locations.
					LocationFinding.TryRandomClosewalkCellNear(pawn, radius, out IntVec3 closeLocation);
					GenSpawn.Spawn(PawnGenerator.GeneratePawn(chosenAnimalDef), closeLocation, map);
				}
			}

			if (availableAnimals.Count == 0)
			{
				Report.Error(
					$"{typeof(PF_GenStep_Animals)} could not finish animal generation because the list of available animals has ended up being empty.");
			}
		}
	}
}