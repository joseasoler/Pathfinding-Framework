using System.Reflection;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.GiddyUp2
{
	/// <summary>
	/// Update rider movement contexts after the map is fully initialized. This is done at this stage because the load
	/// order of riders and mounts is not deterministic.
	/// Since PawnMovementUpdater.Update is fully Giddy-Up 2 compatible, all updates from other sources should be
	/// consistent.
	/// </summary>
	[HarmonyPatch]
	internal static class Pawn_PostMapInit_Patch
	{
		static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(Pawn), nameof(Pawn.PostMapInit));
		}

		private static void Prefix(Pawn __instance)
		{
			if (GiddyUp2Compat.GetMount(__instance) != null)
			{
				PawnMovementUpdater.Update(__instance);
			}
		}
	}
}