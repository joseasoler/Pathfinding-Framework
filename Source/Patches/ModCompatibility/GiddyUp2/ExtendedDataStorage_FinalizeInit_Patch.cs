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
	public static class ExtendedDataStorage_FinalizeInit_Patch
	{
		private const string TypeName = "ExtendedDataStorage";
		private const string MethodName = "FinalizeInit";

		public static bool Prepare(MethodBase original)
		{
			return ModAssemblyInfo.GiddyUp2Assembly != null;
		}

		public static MethodBase TargetMethod()
		{
			return ModCompatibilityUtility.MethodFromAssembly(ModAssemblyInfo.GiddyUp2Assembly, TypeName, MethodName);
		}

		public static void Postfix(object ____store)
		{
			IDictionary store = ____store as IDictionary;
			GiddyUp2Compat.SetGiddyUpStorage(store);
		}
	}
}