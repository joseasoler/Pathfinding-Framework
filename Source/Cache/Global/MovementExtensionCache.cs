using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PathfindingFramework.Cache.Global
{
	/// <summary>
	/// Caches every valid def with a MovementExtension.
	/// The cache is initialized after game load, and never modified afterwards.
	/// </summary>
	public static class MovementExtensionCache
	{
		private static Dictionary<int, MovementDef> _defs;

		private static void AddDefsFromList<TDefType>(List<TDefType> source) where TDefType : Def
		{
			for (int index = 0; index < source.Count; ++index)
			{
				Def currentDef = source[index];
				MovementDef movementDef = currentDef.GetModExtension<MovementExtension>()?.movementDef;
				if (movementDef != null)
				{
					_defs[currentDef.shortHash] = movementDef;
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
				_defs = new Dictionary<int, MovementDef>();

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

		/// <summary>
		/// Check if a Def has a MovementExtension.
		/// </summary>
		/// <param name="def">Def to check.</param>
		/// <returns>true if the Def is in the cache.</returns>
		public static bool Contains(Def def)
		{
			return _defs.ContainsKey(def.shortHash);
		}

		/// <summary>
		/// Get the extension of a Def
		/// </summary>
		/// <param name="def">Def to check.</param>
		/// <returns>MovementDef associated to this Def.</returns>
		public static MovementDef GetMovementDef(Def def)
		{
			return _defs.TryGetValue(def.shortHash, out var result) ? result : null;
		}

		public static List<MemoryUsageData> MemoryReport()
		{
			return new List<MemoryUsageData>
			{
				new(nameof(MovementExtensionCache), MemoryUsageData.Global, "Movement extension Defs",
					_defs.Count * (MemoryUsageData.DictionaryPairSize + sizeof(long)))
			};
		}
	}
}