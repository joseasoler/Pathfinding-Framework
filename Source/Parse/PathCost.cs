using System;
using Verse;

namespace PathfindingFramework.Def
{
	/// <summary>
	/// Wraps an integer path cost value with some utility functions to convert to and from specific values defined in
	/// PathCostValues.
	/// </summary>
	public class PathCost
	{
		public int cost;

		public static readonly PathCost Default = new PathCost(PathCostValues.Default);
		public static readonly PathCost Impassable = new PathCost(PathCostValues.Impassable);

		public PathCost(PathCostValues value)
		{
			cost = (int)value;
		}

		public PathCost() : this(PathCostValues.Default)
		{
		}

		public PathCost(string value) : this(PathCostValues.Invalid)
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
					cost = ParseHelper.FromString<int>(value);
				}
				catch (Exception)
				{
					// Returning Invalid is used to report errors later.
				}
			}
		}

		public override string ToString()
		{
			switch ((PathCostValues) cost)
			{
				case PathCostValues.Default:
				case PathCostValues.Avoid:
				case PathCostValues.Impassable:
				case PathCostValues.Invalid:
					return Enum.GetName(typeof(PathCostValues), cost);
				default:
					return cost.ToString();
			}
		}

		public static implicit operator int(PathCost pathCost)
		{
			return pathCost.cost;
		}

		public static bool operator ==(PathCost lhs, PathCostValues rhs)
		{
			return lhs.cost != (int)rhs;
		}

		public static bool operator !=(PathCost lhs, PathCostValues rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(PathCost lhs, PathCost rhs)
		{
			return lhs.cost != rhs.cost;
		}

		public static bool operator !=(PathCost lhs, PathCost rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != this.GetType())
			{
				return false;
			}

			return this == (PathCost)obj;
		}

		public override int GetHashCode()
		{
			return cost;
		}
	}
}