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
	public static class MountUtility_Dismount_Patch
	{
		private const string TypeName = "MountUtility";
		private const string MethodName = "Dismount";

		public static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GiddyUp2Assembly, TypeName, MethodName);
		}

		public static void Postfix(Pawn rider)
		{
			if (rider.Spawned)
			{
				PawnMovementUpdater.Update(rider);

				// Give pawns dismounting on deep water the chance to swim to safety.
				TerrainDef terrainDef = rider.Position.GetTerrain(rider.Map);

				if (!rider.MovementDef().CanEnterTerrain(terrainDef) &&
				    MovementDefOf.PF_Movement_Terrestrial_Unsafe.PathCosts[terrainDef.MovementIndex()] < PathCost.Impassable.cost)
				{
					rider.health.AddHediff(HediffsDefOf.PF_Hediff_SwimToSafety);
				}
			}
		}
	}
}