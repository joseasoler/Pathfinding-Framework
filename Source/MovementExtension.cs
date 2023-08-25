using System.Collections.Generic;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Define an alternative movement type to use for pawns. This extension is compatible with the following Def types.
	/// These types are listed in order of priority.
	/// * ThingDef: pawns wearing this item as apparel.
	/// * GeneDef: pawns with this gene.
	/// * LifeStageDef: pawns currently in this life stage.
	/// * ThingDef: pawns with this ThingDef as their race.
	/// 
	/// </summary>
	public class MovementExtension : DefModExtension
	{
		/// <summary>
		/// Enable this movement type for this pawn.
		/// </summary>
		public MovementDef movementDef;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (movementDef == null)
			{
				yield return Report.ConfigError(typeof(MovementExtension), "must reference a valid movementDef.");
			}
		}
	}
}