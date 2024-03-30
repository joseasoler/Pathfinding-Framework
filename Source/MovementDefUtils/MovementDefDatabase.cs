using System;
using System.Collections.Generic;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	public static class MovementDefDatabase<TDefType> where TDefType : Def
	{
		private static readonly Dictionary<ushort, MovementDef> movementDefsByDefShortHash = new();

		public static void AddMovementDefs(Func<TDefType, MovementDef, string> validator = null)
		{
			List<TDefType> source = DefDatabase<TDefType>.AllDefsListForReading;
			for (int index = 0; index < source.Count; ++index)
			{
				TDefType currentDef = source[index];
				MovementDef movementDef = currentDef.GetModExtension<MovementExtension>()?.movementDef;
				if (movementDef == null)
				{
					continue;
				}

				movementDefsByDefShortHash[currentDef.shortHash] = movementDef;

				if (validator != null)
				{
					string result = validator(currentDef, movementDef);
					if (!result.NullOrEmpty())
					{
						Report.Error(result);
					}
				}
			}
		}

		public static MovementDef Get(TDefType def)
		{
			return movementDefsByDefShortHash.TryGetValue(def.shortHash, out MovementDef movementDef) ? movementDef : null;
		}
	}
}