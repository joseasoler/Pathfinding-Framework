using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.Patches;
using Verse;

namespace PathfindingFramework.Hediffs
{
	/// <summary>
	/// Class for hediffs which grant a temporary movement until the pawn can move to safety.
	/// </summary>
	public class Hediff_TemporaryMovement : Hediff
	{
		private TerrainDef _previousTerrain;

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(293))
			{
				return;
			}

			TerrainDef terrainDef = pawn.Position.GetTerrain(pawn.Map);
			if (terrainDef != _previousTerrain)
			{
				_previousTerrain = terrainDef;
				if (pawn.MovementDef().CanEnterTerrain(terrainDef))
				{
					pawn.health.RemoveHediff(this);
				}
			}
		}
	}
}