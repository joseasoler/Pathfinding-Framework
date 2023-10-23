using System.Reflection;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.PawnMovement;
using Verse;

namespace PathfindingFramework.Patches.ModCompatibility.GiddyUp2
{
	/// <summary>
	/// Update rider movement context when they start riding.
	/// </summary>
	[HarmonyPatch]
	internal static class MountUtility_Mount_Patch
	{
		private const string TypeName = "MountUtility";
		private const string MethodName = "Mount";

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
			PawnMovementUpdater.Update(rider);
		}
	}
}