using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	using PawnMovementEntry = Tuple<PawnKindDef, Texture>;

	/// <summary>
	/// Sorts entries of the pawn movement table. Assumes that all entries are non-null and have a race.
	/// </summary>
	public class ComparePawnMovementEntries : IComparer<PawnMovementEntry>
	{
		public int Compare(PawnMovementEntry lhsEntry, PawnMovementEntry rhsEntry)
		{
			ThingDef lhs = lhsEntry!.Item1.race;
			ThingDef rhs = rhsEntry!.Item1.race;
			bool humanlikeLhs = lhs.race.Humanlike;
			bool humanlikeRhs = rhs.race.Humanlike;

			int humanlikeCompare = humanlikeLhs.CompareTo(humanlikeRhs);
			if (humanlikeCompare != 0)
			{
				return -humanlikeCompare;
			}

			bool animalLhs = lhs.race.Animal;
			bool animalRhs = rhs.race.Animal;

			int animalCompare = animalLhs.CompareTo(animalRhs);
			if (animalCompare != 0)
			{
				return -animalCompare;
			}

			return string.Compare(lhs.LabelCap.ToString(), rhs.LabelCap.ToString(), StringComparison.Ordinal);
		}
	}
}