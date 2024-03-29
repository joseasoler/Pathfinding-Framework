﻿using System.Collections.Generic;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// A movement type is defined by a set of custom path costs and rules that pawns must follow.
	/// </summary>
	public class MovementDef : Verse.Def
	{
		/// <summary>
		/// Used to choose when two or more movement types are available.
		/// </summary>
		public short priority;

		/// <summary>
		/// Maps terrain tags to their path costs in this movement type.
		/// Setting a value lower than 10000 for an impassable terrain will let it be passable for this movement type.
		/// If a terrain has more than one matching tag, the largest tag value will be used.
		/// </summary>
		public TerrainTagPathCosts tagCosts;

		/// <summary>
		/// Default path cost for terrains that are not affected by other movement type changes. Cannot make impassable
		/// terrains passable.
		/// </summary>
		public PathCost defaultCost;

		/// <summary>
		/// Passable terrains not affected by other movement type changes will get this value added to their path cost.
		/// </summary>
		public short defaultCostAdd = -1;

		/// <summary>
		/// When this flag is set, this movement type will not consider snow path costs.
		/// </summary>
		public bool ignoreSnow;

		/// <summary>
		/// When this flag is set, this movement type will ignore thing path costs except for impassable ones.
		/// </summary>
		public bool ignoreThings;

		/// <summary>
		/// Marks this movement type as not intended for penned animals. When this flag is set, all pawns will treat fences
		/// normally even if they are set to avoid them. A warning will be displayed when penned animals use this movement
		/// type.
		/// </summary>
		public bool penAnimalsDisallowed;

		/// <summary>
		/// When this flag is set, animals with this movement type can only appear as manhunters on world tiles with access
		/// to water. This requires a river, a coast, or a biome covered in water (such as ocean or Biomes! Islands).
		/// </summary>
		public bool manhuntersRequireWater;

		/// <summary>
		/// If this flag is set, animals with this movement type will be able to spawn in points from which it is not
		/// possible to reach the colony.
		/// </summary>
		public bool ignoreColonyReachability;

		/// <summary>
		/// When this flag is set, pawns ignore the "avoidWander" value of vanilla terrains.
		/// </summary>
		public bool ignoreAvoidWander;

		/// <summary>
		/// Stores a precalculated list of all terrain path costs for this movement type.
		/// This is initialized after XML references are resolved, and never modified afterwards.
		/// </summary>
		public short[] PathCosts = null;

		/// <summary>
		/// Checks if pawns with this movement should be able to choose a terrain as a valid position.
		/// </summary>
		/// <param name="terrainDef">Terrain to check.</param>
		/// <returns>True if the terrain is passable.</returns>
		public bool CanEnterTerrain(TerrainDef terrainDef)
		{
			int cost = PathCosts[terrainDef.MovementIndex()];
			return cost < PathCost.Unsafe.cost;
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}

			if (defaultCost.cost >= PathCost.Impassable.cost)
			{
				yield return Report.ConfigError(this,
					"defaultCost must be a numeric value or a valid PathingCost smaller than Impassable.");
			}

			if (tagCosts != null)
			{
				foreach (var tagCost in tagCosts.data)
				{
					if (tagCost.Value == PathCost.Invalid)
					{
						yield return Report.ConfigError(this,
							$"tagCost {tagCost.Key} must be a numeric value or a valid PathingCost. But it was {tagCost.Value}");
					}
				}
			}
		}

		public override void ResolveReferences()
		{
			if (descriptionHyperlinks != null)
			{
				return;
			}

			descriptionHyperlinks = Hyperlinks.Get(this);
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}

			foreach (StatDrawEntry item in StatDrawEntries.Get(this))
			{
				yield return item;
			}
		}
	}
}