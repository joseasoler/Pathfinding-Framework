using HarmonyLib;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.ModExtensions.TerrainTagGraphic
{
	/// <summary>
	/// Detect when a pawn is standing on a new terrain type.
	/// Update terrain tag graphics if required.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_PathFollower), "TryEnterNextPathCell")]
	internal static class Pawn_PathFollower_TryEnterNextPathCell_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			if (___pawn == null || !___pawn.Spawned)
			{
				return;
			}

			TerrainDef currentTerrainDef = ___pawn.Position.GetTerrain(___pawn.Map);
			TerrainDef previousTerrainDef = ___pawn.CurrentTerrainDef();
			if (currentTerrainDef == previousTerrainDef)
			{
				return;
			}

			___pawn.CurrentTerrainDef() = currentTerrainDef;
			___pawn.GraphicContext()?.TerrainUpdated(previousTerrainDef, currentTerrainDef);
		}
	}
}