using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.Cache
{
	/// <summary>
	/// Keeps track of the MovementDef associated with each spawned pawn. See MovementExtension for details.
	/// </summary>
	public static class PawnMovementCache
	{
		/// <summary>
		/// Indexed by [pawn.thingIDNumber, movementDef.index].
		/// </summary>
		private static readonly Dictionary<int, int> MovementByPawn = new Dictionary<int, int>();

		/// <summary>
		/// Add all movement definitions obtained from apparel.
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
				var extension = MovementExtensionCache.GetExtension(apparelDef);
				if (extension != null)
				{
					movementDefs.Add(extension.movementDef);
				}
			}
		}

		/// <summary>
		/// Check MovementExtensions of the pawn's genes and return the one with the largest sameTypePriority.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromGenes(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			var geneList = pawn.genes.GenesListForReading;

			for (int index = 0; index < geneList.Count; ++index)
			{
				var gene = geneList[index];
				if (!gene.Active)
				{
					continue;
				}

				var extension = MovementExtensionCache.GetExtension(gene.def);
				if (extension != null)
				{
					movementDefs.Add(extension.movementDef);
				}
			}
		}

		/// <summary>
		/// Get the MovementDef of the current life stage, if any.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated.</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromLifeStage(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			var movementDef = MovementExtensionCache.GetExtension(pawn.ageTracker?.CurLifeStage)?.movementDef;
			if (movementDef != null)
			{
				movementDefs.Add(movementDef);
			}
		}

		/// <summary>
		/// Get the MovementDef of the ThingDef associated with this pawn.
		/// </summary>
		/// <param name="pawn">Pawn being evaluated.</param>
		/// <param name="movementDefs">Set of movement definitions available to the pawn.</param>
		private static void FromRace(Pawn pawn, ref HashSet<MovementDef> movementDefs)
		{
			var movementDef = MovementExtensionCache.GetExtension(pawn.def)?.movementDef;
			if (movementDef != null)
			{
				movementDefs.Add(movementDef);
			}
		}

		/// <summary>
		/// Set the MovementDef to use for this pawn. See MovementExtension for detail.
		/// </summary>
		/// <param name="pawn">Pawn to evaluate.</param>
		public static void Recalculate(Pawn pawn)
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

			MovementByPawn[pawn.thingIDNumber] = movementDef?.index ?? MovementDefOf.PF_Terrestrial.index;
		}

		public static int Get(Pawn pawn)
		{
			if (MovementByPawn.TryGetValue(pawn.thingIDNumber, out var result))
			{
				return result;
			}
			
			Report.ErrorOnce($"Pawn {pawn.ThingID} is missing from the {nameof(PawnMovementCache)} cache.");
			return MovementDefOf.PF_Terrestrial.index;
		}
	}
}