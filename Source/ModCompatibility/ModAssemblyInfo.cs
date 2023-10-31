using System.Reflection;
using Verse;

namespace PathfindingFramework.ModCompatibility
{
	/// <summary>
	/// Checks presence of other mods and stores their assemblies to be used for compatibility patches.
	/// </summary>
	public static class ModAssemblyInfo
	{
		/// <summary>
		/// Giddy-Up 2
		/// https://steamcommunity.com/sharedfiles/filedetails/?id=2934245647
		/// </summary>
		public static Assembly GiddyUp2Assembly;

		/// <summary>
		/// Vanilla Furniture Expanded - Security
		/// https://steamcommunity.com/sharedfiles/filedetails/?id=1845154007
		/// </summary>
		public static Assembly VanillaFurnitureExpandedSecurity;

		public static void Initialize()
		{
			foreach (var pack in LoadedModManager.RunningMods)
			{
				string packageId = pack.PackageId.ToLower();
				switch (packageId)
				{
					case "owlchemist.giddyup":
						GiddyUp2Assembly = pack.assemblies.loadedAssemblies[0];
						break;
					case "vanillaexpanded.vfesecurity":
						VanillaFurnitureExpandedSecurity = pack.assemblies.loadedAssemblies[0];
						break;
				}
			}
		}
	}
}