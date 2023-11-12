using System;
using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Generate hyperlinks for a specific movement type.
	/// </summary>
	public static class Hyperlinks
	{
		private static void AddDefHyperlinksFromType<TDefType>(MovementDef movementDef, ref List<DefHyperlink> hyperlinks,
			Func<TDefType, bool> condition = null) where TDefType : Def
		{
			List<Def> defsWithThisMovement = DefsWithMovementType.Get<TDefType>(movementDef, condition);
			foreach (Def result in defsWithThisMovement)
			{
				hyperlinks.Add(new DefHyperlink(result));
			}
		}

		public static List<DefHyperlink> Get(MovementDef movementDef)
		{
			List<DefHyperlink> hyperlinks = new List<DefHyperlink>();

			AddDefHyperlinksFromType<ThingDef>(movementDef, ref hyperlinks, DefsWithMovementType.IsCreature);
			AddDefHyperlinksFromType<HediffDef>(movementDef, ref hyperlinks);
			AddDefHyperlinksFromType<ThingDef>(movementDef, ref hyperlinks, DefsWithMovementType.IsApparel);

			if (ModLister.BiotechInstalled)
			{
				AddDefHyperlinksFromType<GeneDef>(movementDef, ref hyperlinks);
			}

			return hyperlinks;
		}
	}
}