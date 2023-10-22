using System.Reflection;
using Verse;

namespace PathfindingFramework
{
	public static class ModAssemblyInfo
	{
		public static Assembly GiddyUp2Assembly;

		public static void Initialize()
		{
			foreach (var pack in LoadedModManager.RunningMods)
			{
				string packageId = pack.PackageId.ToLower();
				if (packageId == "owlchemist.giddyup")
				{
					GiddyUp2Assembly = pack.assemblies.loadedAssemblies[0];
				}
			}
		}
	}
}