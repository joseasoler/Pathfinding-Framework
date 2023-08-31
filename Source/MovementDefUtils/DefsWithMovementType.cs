using System;
using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	public static class DefsWithMovementType
	{
		public static List<Def> Get<TDefType>(MovementDef movementDef,
			Func<TDefType, bool> condition = null)
			where TDefType : Def
		{
			var defs = DefDatabase<TDefType>.AllDefsListForReading;
			var resultList = new List<Def>();
			for (int index = 0; index < defs.Count; ++index)
			{
				var currentDef = defs[index];
				var extension = currentDef.GetModExtension<MovementExtension>();
				if (!currentDef.generated && extension != null && extension.movementDef == movementDef &&
				    (condition == null || condition(currentDef)))
				{
					resultList.Add(currentDef);
				}
			}

			return resultList;
		}

		public static bool IsApparel(ThingDef thingDef)
		{
			return thingDef.IsApparel;
		}

		public static bool IsCreature(ThingDef thingDef)
		{
			return thingDef.race != null;
		}
	}
}