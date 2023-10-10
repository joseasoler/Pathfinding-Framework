using System;
using Verse;

namespace PathfindingFramework.Parse
{
	/// <summary>
	/// Wraps an integer path cost value with some utility functions to convert to and from specific values defined in
	/// PathCostValues.
	/// </summary>
	public struct PathCost
	{

		/// <summary>
		/// Final integer path cost associated with this instance.
		/// </summary>
		public short cost;

		/// <summary>
		/// PathCost with a value of PathCostValues.Invalid, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Invalid = new((short)PathCostValues.Invalid);

		/// <summary>
		/// PathCost with a value of PathCostValues.Default, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Default = new((short)PathCostValues.Default);

		/// <summary>
		/// PathCost with a value of PathCostValues.Avoid, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Avoid = new((short)PathCostValues.Avoid);

		/// <summary>
		/// PathCost with a value of PathCostValues.Unsafe, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Unsafe = new((short)PathCostValues.Unsafe);

		/// <summary>
		/// PathCost with a value of PathCostValues.Impassable, pre-initialized for performance reasons.
		/// </summary>
		public static readonly PathCost Impassable = new((short)PathCostValues.Impassable);

		/// <summary>
		/// Initialize directly from a numerical value.
		/// </summary>
		/// <param name="value">Integer value to use for the cost.</param>
		public PathCost(short value)
		{
			cost = value;
		}

		/// <summary>
		/// Parse a path cost from a string into their numerical cost.
		/// </summary>
		/// <param name="value">One of the PathCostValues or positive integer.</param>
		public PathCost(string value) : this(Invalid.cost)
		{
			try
			{
				cost = (short)Enum.Parse(typeof(PathCostValues), value);
			}
			catch (Exception)
			{
				// value can be one of the values defined in the PathfindingCost enum, but it can also be an integer value.
				try
				{
					short parsedCost = ParseHelper.FromString<short>(value);
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
		
		public static bool operator ==(PathCost lhs, PathCost rhs)
		{
			return lhs.cost == rhs.cost;
		}

		public static bool operator !=(PathCost lhs, PathCost rhs)
		{
			return lhs.cost != rhs.cost;
		}

		private bool Equals(PathCost other)
		{
			return cost == other.cost;
		}

		public override bool Equals(object obj)
		{
			return obj is PathCost other && Equals(other);
		}

		public override int GetHashCode()
		{
			return cost.GetHashCode();
		}
	}
}