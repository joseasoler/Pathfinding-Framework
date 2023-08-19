using System.Collections.Generic;
using PathfindingFramework.Def;

namespace PathfindingFramework
{
	/// <summary>
	/// A movement type is defined by a set of custom pathing rules that pawns must follow.
	/// </summary>
	public class MovementTypeDef : Verse.Def
	{
		/// <summary>
		/// Customize the path cost of specific terrain tags for this movement type.
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


		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			if (defaultCost == PathCost.Invalid)
			{
				yield return Report.ConfigError(this, $"defaultCost must be a numeric value or a valid PathingCost.");
			}

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
}