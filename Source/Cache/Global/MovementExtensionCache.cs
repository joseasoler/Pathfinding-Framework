using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PathfindingFramework.Cache
{
	/// <summary>
	/// Caches every valid def with a MovementExtension.
	/// The cache is initialized after game load, and never modified afterwards.
	/// </summary>
	public static class MovementExtensionCache
	{
		private static Dictionary<ushort, MovementExtension> _defs;

		private static void AddDefsFromList<TDefType>(List<TDefType> source) where TDefType : Def
		{
			for (int index = 0; index < source.Count; ++index)
			{
				Def currentDef = source[index];
				var extension = currentDef.GetModExtension<MovementExtension>();
				if (extension != null)
				{
					_defs[currentDef.shortHash] = extension;
				}
			}
		}

		/// <summary>
		/// Gather the defs with a MovementExtension after game load.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				_defs = new Dictionary<ushort, MovementExtension>();

				AddDefsFromList(DefDatabase<ThingDef>.AllDefsListForReading);
				AddDefsFromList(DefDatabase<LifeStageDef>.AllDefsListForReading);
				AddDefsFromList(DefDatabase<HediffDef>.AllDefsListForReading);
				if (ModLister.BiotechInstalled)
				{
					AddDefsFromList(DefDatabase<GeneDef>.AllDefsListForReading);
				}

				Report.Debug("Movement type by Def cache initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Movement type initialization failed:");
				Report.Error($"{exception}");
			}
		}

		public static bool Contains(Def def)
		{
			return _defs.ContainsKey(def.shortHash);
		}

		public static MovementExtension GetExtension(Def def)
		{
			return _defs.TryGetValue(def.shortHash, out var result) ? result : null;
		}
	}
}