using System.Collections.Generic;
using PathfindingFramework.Cache.Global;
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
			var apparelList = pawn.apparel?.WornApparel;
			if (apparelList == null)
			{
				return;
			}

			for (int index = 0; index < apparelList.Count; ++index)
			{
				var apparelDef = apparelList[index].def;
				var movementDef = MovementExtensionCache.GetMovementDef(apparelDef);
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
			var geneList = pawn.genes?.GenesListForReading;
			if (geneList == null)
			{
				return;
			}

			for (int index = 0; index < geneList.Count; ++index)
			{
				var gene = geneList[index];
				if (!gene.Active)
				{
					continue;
				}

				MovementDef movementDef = MovementExtensionCache.GetMovementDef(gene.def);
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
			var hediffList = pawn.health?.hediffSet?.hediffs;
			if (hediffList == null)
			{
				return;
			}

			for (int index = 0; index < hediffList.Count; ++index)
			{
				var hediff = hediffList[index];

				MovementDef movementDef = MovementExtensionCache.GetMovementDef(hediff.def);
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
			MovementDef movementDef = MovementExtensionCache.GetMovementDef(lifeStageDef);
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
			var movementDef = MovementExtensionCache.GetMovementDef(pawn.def);
			if (movementDef != null)
			{
				movementDefs.Add(movementDef);
			}
		}

		/// <summary>
		/// Updates the MovementDef to use for this pawn. See MovementExtension for details.
		/// </summary>
		/// <param name="pawn">Pawn to evaluate.</param>
		public static void Update(Pawn pawn)
		{
			/*
			 * Recalculate this. Now, MovementDef has a priority. And MovementExtension can combine.
			 * Also do canCombine in the MovementExtensions for testing.
			 */
			var movementDefs = new HashSet<MovementDef>();
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

			foreach (var currentDef in movementDefs)
			{
				if (priority < currentDef.priority)
				{
					priority = currentDef.priority;
					movementDef = currentDef;
				}
			}

			movementDef ??= MovementDefOf.PF_Terrestrial;
			pawn.MovementDef() = movementDef;
			pawn.Map.MovementContextData().UpdatePawn(pawn);
		}
	}
}