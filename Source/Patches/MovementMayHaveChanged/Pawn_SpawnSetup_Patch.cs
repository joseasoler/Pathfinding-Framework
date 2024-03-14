using System.Collections.Generic;
using HarmonyLib;
using PathfindingFramework.PawnGraphic;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.MovementMayHaveChanged
{
	/// <summary>
	/// Update the movement type of the pawn in the PawnMovementCache.
	/// Initialize a new graphic context if necessary.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	public static class Pawn_SpawnSetup_Patch
	{
		public static void Postfix(Pawn __instance)
		{
			PawnMovementUpdater.Update(__instance);
			// Used to detect current terrain type changes.
			__instance.CurrentTerrainDef() = __instance.Position.GetTerrain(__instance.Map);

			List<DefModExtension> extensions = __instance.kindDef.modExtensions;
			if (extensions == null)
			{
				return;
			}

			bool createContext = false;
			for (int extensionIndex = 0; extensionIndex < extensions.Count; ++extensionIndex)
			{
				DefModExtension extension = extensions[extensionIndex];
				if (extension is LocomotionGraphicExtension || extension is TerrainTagGraphicExtension)
				{
					createContext = true;
					break;
				}
			}

			Report.Error($"Pawn_SpawnSetup_Patch createContext for {__instance} -> {createContext}");
			if (createContext)
			{
				__instance.GraphicContext() = new GraphicContext(__instance);
			}
		}
	}
}