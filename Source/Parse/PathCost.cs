using System;
using Verse;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Wraps an integer path cost value with some utility functions to convert to and from specific values defined in
	/// PathCostValues.
	/// </summary>
	public record struct PathCost
	{
		/// <summary>
		/// Final integer path cost associated with this instance.
		/// </summary>
		public int cost;

		/// <summary>
		/// PathCost with a value of PathCostValues.Invalid, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Invalid = new PathCost((int)PathCostValues.Invalid);

		/// <summary>
		/// PathCost with a value of PathCostValues.Default, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Default = new PathCost((int)PathCostValues.Default);

		/// <summary>
		/// PathCost with a value of PathCostValues.Impassable, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Impassable = new PathCost((int)PathCostValues.Impassable);

		/// <summary>
		/// Initialize directly from a numerical value. Only intended for internal use in this class.
		/// </summary>
		/// <param name="value">Integer value to use for the cost.</param>
		private PathCost(int value)
		{
			cost = value;
		}

		/// <summary>
		/// By default, PathCoths are initialized to a special value. In this case, the cost defined by the TerrainDef
		/// itself will be used.
		/// </summary>
		public PathCost() : this(Default.cost)
		{
		}

		/// <summary>
		/// Parse a path cost from a string into their numerical cost.
		/// </summary>
		/// <param name="value">One of the PathCostValues or positive integer.</param>
		public PathCost(string value) : this(Invalid.cost)
		{
			try
			{
				cost = (int)Enum.Parse(typeof(PathCostValues), value);
			}
			catch (Exception)
			{
				// value can be one of the values defined in the PathfindingCost enum, but it can also be an integer value.
				try
				{
					var parsedCost = ParseHelper.FromString<int>(value);
					if (parsedCost >= 0)
					{
						cost = parsedCost;
					}
				}
				catch (Exception)
				{
					// Returning Invalid is used to report errors later.
				}
			}
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(PathCostValues), cost) ?? cost.ToString();
		}
	}
}