using System;
using System.Collections.Generic;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Applies a movement speed multiplier when the pawn is standing in a terrain with one of the terrain tags.
	/// This extension must be applied to the ThingDef of the pawn.
	/// </summary>
	public class TerrainTagMovementExtension : DefModExtension
	{
		/// <summary>
		/// List of terrain tags that will apply a speed multiplier.
		/// </summary>
		public List<string> terrainTags = new();

		/// <summary>
		/// Movement speed multiplier to apply when standing over one of the terrain tags.
		/// </summary>
		public float moveSpeedMultiplier = 1.0F;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (terrainTags.NullOrEmpty())
			{
				yield return Report.ConfigError(typeof(TerrainTagMovementExtension), "terrainTags must not be empty.");
			}

			if (Math.Abs(moveSpeedMultiplier - 1.0F) < 0.00001F)
			{
				yield return Report.ConfigError(typeof(TerrainTagMovementExtension),
					"must have a moveSpeedMultiplier multiplier different than 1.");
			}
		}

		public bool Affects(TerrainDef terrainDef)
		{
			if (terrainDef?.tags == null)
			{
				return false;
			}

			List<string> terrainDefTags = terrainDef.tags;
			for (int index = 0; index < terrainDefTags.Count; ++index)
			{
				if (terrainTags.Contains(terrainDefTags[index]))
				{
					return true;
				}
			}

			return false;
		}
	}
}