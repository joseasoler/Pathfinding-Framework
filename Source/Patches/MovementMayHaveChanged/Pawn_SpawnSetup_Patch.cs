using HarmonyLib;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the movement type of the pawn in the PawnMovementCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	internal static class Pawn_SpawnSetup_Patch
	{
		internal static void Postfix(Pawn __instance)
		{
			PawnMovementUpdater.Update(__instance);

			__instance.LocomotionExtension() = __instance.def.HasModExtension<LocomotionExtension>()
				? __instance.def.GetModExtension<LocomotionExtension>()
				: null;
			__instance.TerrainTagGraphicExtension() = __instance.def.HasModExtension<TerrainTagGraphicExtension>()
				? __instance.def.GetModExtension<TerrainTagGraphicExtension>()
				: null;
			__instance.CurrentTerrainDef() = __instance.Position.GetTerrain(__instance.Map);
		}
	}
}