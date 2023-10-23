using System.Collections;
using System.Reflection;
using HarmonyLib;
using PathfindingFramework.ModCompatibility;

namespace PathfindingFramework.Patches.ModCompatibility.GiddyUp2
{
	/// <summary>
	/// Takes a reference to the Giddy-Up data cache to be stored in the Pathfinding Framework code base.
	/// </summary>
	[HarmonyPatch]
	internal static class ExtendedDataStorage_FinalizeInit_Patch
	{
		private const string TypeName = "ExtendedDataStorage";
		private const string MethodName = "FinalizeInit";

		private static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		private static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GiddyUp2Assembly, TypeName, MethodName);
		}

		private static void Postfix(object ____store)
		{
			IDictionary store = ____store as IDictionary;
			GiddyUp2Compat.SetGiddyUpStorage(store);
		}
	}
}