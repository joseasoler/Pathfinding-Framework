using System;
using System.Collections.Generic;
using PathfindingFramework.Patches;
using RimWorld;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Caches every valid def with a MovementExtension.
	/// The cache is initialized after game load, and never modified afterwards.
	/// </summary>
	public static class MovementExtensionReader
	{
		private static void AddDefsFromList<TDefType>(List<TDefType> source, Action<TDefType, MovementDef> setMovement,
			Func<TDefType, MovementDef, string> validator = null)
			where TDefType : Def
		{
			for (int index = 0; index < source.Count; ++index)
			{
				TDefType currentDef = source[index];
				MovementDef movementDef = currentDef.GetModExtension<MovementExtension>()?.movementDef;
				if (movementDef == null)
				{
					continue;
				}

				setMovement(currentDef, movementDef);

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

		private static void SetThingDefMovement(ThingDef thingDef, MovementDef movementDef)
		{
			thingDef.MovementDef() = movementDef;
			// Keep the original value to use it for settings reset actions.
			PawnMovementOverrideSettings.AddOriginal(thingDef, movementDef);
		}

		private static void SetLifeStageDefMovement(LifeStageDef lifeStageDef, MovementDef movementDef)
		{
			lifeStageDef.MovementDef() = movementDef;
		}

		private static void SetHediffDefMovement(HediffDef lifeStageDef, MovementDef movementDef)
		{
			lifeStageDef.MovementDef() = movementDef;
		}

		private static void SetGeneDefMovement(GeneDef lifeStageDef, MovementDef movementDef)
		{
			lifeStageDef.MovementDef() = movementDef;
		}

		private static string ValidateThingDef(ThingDef thingDef, MovementDef movementDef)
		{
			if (!movementDef.penAnimalsDisallowed || thingDef.race == null || !thingDef.race.FenceBlocked)
			{
				return null;
			}

			string packageId = thingDef.modContentPack != null
				? thingDef.modContentPack.PackageIdPlayerFacing
				: "Unknown".Translate();

			return movementDef.penAnimalsDisallowed && thingDef.race != null && thingDef.race.FenceBlocked
				? $"{thingDef.defName}[{packageId}] is a roamer, but has been assigned movement type {movementDef.defName} which disables pen animal pathfinding."
				: null;
		}

		/// <summary>
		/// Gather the defs with a MovementExtension after game load.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				AddDefsFromList(DefDatabase<ThingDef>.AllDefsListForReading, SetThingDefMovement, ValidateThingDef);
				AddDefsFromList(DefDatabase<LifeStageDef>.AllDefsListForReading, SetLifeStageDefMovement);
				AddDefsFromList(DefDatabase<HediffDef>.AllDefsListForReading, SetHediffDefMovement);
				if (ModLister.BiotechInstalled)
				{
					AddDefsFromList(DefDatabase<GeneDef>.AllDefsListForReading, SetGeneDefMovement);
				}

				Report.Debug("Movement type by Def cache initialization complete.");
			}
			catch (Exception exception)
			{
				Report.Error("Movement type initialization failed:");
				Report.Error($"{exception}");
			}
		}
	}
}