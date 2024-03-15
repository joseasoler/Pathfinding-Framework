using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.Patches;
using RimWorld;
using Verse;

namespace PathfindingFramework.PawnMovement
{
	/// <summary>
	/// Updates the MovementDef associated with each spawned pawn as needed. See MovementExtension for details.
	/// </summary>
	public static class PawnMovementUpdater
	{
		/// <summary>
		/// Add all movement definitions obtained from apparel to the set.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		/// <returns>MovementExtension granted by apparel.</returns>
		private static void FromApparel(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			List<Apparel> apparelList = pawn.apparel?.WornApparel;
			if (apparelList == null)
			{
				return;
			}

			for (int apparelIndex = 0; apparelIndex < apparelList.Count; ++apparelIndex)
			{
				ThingDef apparelDef = apparelList[apparelIndex].def;
				MovementDef movementDef = apparelDef.MovementDef();
				if (movementDef != null)
				{
					movementDefs.Add(movementDef);
				}
			}
		}

		/// <summary>
		/// Add MovementExtensions from the pawn genes to the set.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromGenes(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			List<Gene> geneList = pawn.genes?.GenesListForReading;
			if (geneList == null)
			{
				return;
			}

			for (int geneIndex = 0; geneIndex < geneList.Count; ++geneIndex)
			{
				var gene = geneList[geneIndex];
				if (!gene.Active)
				{
					continue;
				}

				MovementDef movementDef = gene.def.MovementDef();
				if (movementDef != null)
				{
					movementDefs.Add(movementDef);
				}
			}
		}

		/// <summary>
		/// Add the MovementExtensions of the pawn's hediffs to the set.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromHediffs(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			List<Hediff> hediffList = pawn.health?.hediffSet?.hediffs;
			if (hediffList == null)
			{
				return;
			}

			for (int hediffIndex = 0; hediffIndex < hediffList.Count; ++hediffIndex)
			{
				var hediff = hediffList[hediffIndex];

				MovementDef movementDef = hediff.def.MovementDef();
				if (movementDef != null)
				{
					movementDefs.Add(movementDef);
				}
			}
		}

		/// <summary>
		/// Add the MovementDef of the current life stage (if any) to the set.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated.</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromLifeStage(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			LifeStageDef lifeStageDef = pawn.ageTracker?.CurLifeStage;
			MovementDef movementDef = lifeStageDef.MovementDef();
			if (movementDef != null)
			{
				movementDefs.Add(movementDef);
			}
		}

		/// <summary>
		/// Add the MovementDef of the ThingDef associated with this pawn, if any.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated.</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromRace(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			MovementDef overrideMovementDef = PawnMovementOverrideSettings.CurrentMovementDef(pawn.def);
			if (overrideMovementDef != null)
			{
				movementDefs.Add(overrideMovementDef);
				return;
			}

			MovementDef movementDef = pawn.def.MovementDef();
			if (movementDef != null)
			{
			}
		}

		/// <summary>
		/// Compatibility with Giddy-Up 2. Finds out if the pawn is a rider. In that case, it copies the MovementDef
		/// and MovementContext of the mount.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>True if the pawn is a rider and the values from the mount have been copied successfully.</returns>
		private static bool TryGetFromMount(Pawn pawn)
		{
			Pawn mount = GiddyUp2Compat.GetMount(pawn);
			if (mount?.MovementContext() != null)
			{
				pawn.MovementDef() = mount.MovementDef();
				pawn.MovementContext() = mount.MovementContext();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Updates the MovementDef to use for this pawn. See MovementExtension for details.
		/// </summary>
		/// <param name="pawn">Pawn to evaluate.</param>
		public static void Update(Pawn pawn)
		{
			if (TryGetFromMount(pawn))
			{
				// If the pawn is riding a mount, it will just use the MovementContext of its mount.
				return;
			}

			HashSet<MovementDef> movementDefs = new HashSet<MovementDef>();
			FromApparel(pawn, ref movementDefs);

			if (ModLister.BiotechInstalled && pawn.genes != null)
			{
				FromGenes(pawn, ref movementDefs);
			}

			FromHediffs(pawn, ref movementDefs);
			FromLifeStage(pawn, ref movementDefs);
			FromRace(pawn, ref movementDefs);

			int priority = int.MinValue;
			MovementDef movementDef = null;

			foreach (MovementDef currentDef in movementDefs)
			{
				if (priority < currentDef.priority)
				{
					priority = currentDef.priority;
					movementDef = currentDef;
				}
			}

			movementDef ??= MovementDefOf.PF_Movement_Terrestrial;
			pawn.MovementDef() = movementDef;
			pawn.Map.MovementContextData().UpdatePawn(pawn);
		}
	}
}