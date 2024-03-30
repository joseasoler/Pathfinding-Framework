using System;
using RimWorld;
using Verse;

namespace PathfindingFramework.MovementDefUtils
{
	/// <summary>
	/// Caches every valid def with a MovementExtension.
	/// The cache is initialized after game load, and never modified afterwards.
	/// </summary>
	public static class MovementExtensionCache
	{
		private static string ValidateThingDef(ThingDef thingDef, MovementDef movementDef)
		{
			if (!movementDef.penAnimalsDisallowed || thingDef.race == null || !thingDef.race.FenceBlocked)
			{
				return null;
			}

			string packageId = thingDef.modContentPack != null
				? thingDef.modContentPack.PackageIdPlayerFacing
				: Translations.Unknown;

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
				MovementDefDatabase<ThingDef>.AddMovementDefs(ValidateThingDef);
				MovementDefDatabase<LifeStageDef>.AddMovementDefs();
				MovementDefDatabase<HediffDef>.AddMovementDefs();
				if (ModLister.BiotechInstalled)
				{
					MovementDefDatabase<GeneDef>.AddMovementDefs();
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