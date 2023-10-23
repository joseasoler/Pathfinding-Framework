using System.Reflection;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.Parse;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.GiddyUp2
{
	/// <summary>
	/// Update rider movement context when they dismount.
	/// </summary>
	[HarmonyPatch]
	internal static class MountUtility_Dismount_Patch
	{
		private const string TypeName = "MountUtility";
		private const string MethodName = "Dismount";

		private static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		private static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GiddyUp2Assembly, TypeName, MethodName);
		}

		private static void Postfix(Pawn rider)
		{
			if (rider.Spawned)
			{
				PawnMovementUpdater.Update(rider);

				// Give pawns dismounting on deep water the chance to swim to safety.
				TerrainDef terrainDef = rider.Position.GetTerrain(rider.Map);

				if (!rider.MovementDef().CanEnterTerrain(terrainDef) &&
				    MovementDefOf.PF_Movement_Terrestrial_Unsafe.PathCosts[terrainDef.index] < PathCost.Impassable.cost)
				{
					rider.health.AddHediff(HediffsDefOf.PF_Hediff_SwimToSafety);
				}
			}
		}
	}
}