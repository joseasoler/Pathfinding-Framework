using System.Reflection;
using HarmonyLib;
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

		static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GiddyUp2Assembly, TypeName, MethodName);
		}

		private static void Postfix(Pawn rider)
		{
			if (rider.Spawned)
			{
				PawnMovementUpdater.Update(rider);
			}
		}
	}
}