using System;
using System.Collections.Generic;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.Parse;
using RimWorld;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// A movement type is defined by a set of custom pathing rules that pawns must follow.
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
		/// By default, passable terrains not affected by other movement type changes will use the costs defined in their
		/// defs. If this field is set to a value equal or larger than zero, this will be the default path cost instead.
		/// defaultPathCost cannot make impassable terrain passable.
		/// </summary>
		public PathCost defaultCost;

		/// <summary>
		/// When this flag is set, this movement type will ignore snow path costs.
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
		public bool noPennedAnimals;

		/// <summary>
		/// Stores a precalculated list of all terrain path costs for this movement type.
		/// This is initialized after XML references are resolved, and never modified afterwards.
		/// </summary>
		public short[] PathCosts = null;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			if (defaultCost == PathCost.Invalid)
			{
				yield return Report.ConfigError(this, "defaultCost must be a numeric value or a valid PathingCost.");
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
			if (PathCosts != null)
			{
				return;
			}

			PathCosts = MovementDefUtils.PathCosts.Get(this);
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