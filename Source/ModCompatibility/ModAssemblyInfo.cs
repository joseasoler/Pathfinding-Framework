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
		/// Prepatcher
		/// https://steamcommunity.com/workshop/filedetails/?id=2934420800
		/// </summary>
		public static bool PrepatcherPresent;

		/// <summary>
		/// Dubs Bad Hygiene
		/// https://steamcommunity.com/workshop/filedetails/?id=836308268
		/// </summary>
		public static Assembly DubsBadHygieneAssembly;

		/// <summary>
		/// Geological Landforms
		/// https://steamcommunity.com/sharedfiles/filedetails/?id=2773943594
		/// </summary>
		public static Assembly GeologicalLandformsAssembly;

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

		/// <summary>
		/// Windows
		/// https://steamcommunity.com/sharedfiles/filedetails/?id=2571189146
		/// </summary>
		public static Assembly Windows;

		public static void Initialize()
		{
			foreach (ModContentPack pack in LoadedModManager.RunningMods)
			{
				if (pack.assemblies.loadedAssemblies.NullOrEmpty())
				{
					continue;
				}

				string packageId = pack.PackageId.ToLower();
				Assembly firstAssembly = pack.assemblies.loadedAssemblies[0];
				switch (packageId)
				{
					case "dubwise.dubsbadhygiene":
						foreach (Assembly assembly in pack.assemblies.loadedAssemblies)
						{
							if (assembly.GetName().Name == "BadHygiene")
							{
								DubsBadHygieneAssembly = assembly;
								break;
							}
						}

						break;
					case "owlchemist.giddyup":
						GiddyUp2Assembly = firstAssembly;
						break;
					case "owlchemist.windows":
						Windows = firstAssembly;
						break;
					case "vanillaexpanded.vfesecurity":
						VanillaFurnitureExpandedSecurity = firstAssembly;
						break;
					case "zetrith.prepatcher":
						PrepatcherPresent = true;
						break;
				}
			}
			Report.Debug("Gathered mod assembly information.");
		}

		/// <summary>
		/// Obtain the assemblies of mods that load additional DLLs at different gama load stages such as Geological
		/// Landforms.
		/// </summary>
		public static void LoadingFinished()
		{
			foreach (ModContentPack pack in LoadedModManager.RunningMods)
			{
				if (pack.assemblies.loadedAssemblies.NullOrEmpty())
				{
					continue;
				}

				string packageId = pack.PackageId.ToLower();
				switch (packageId)
				{
					case "m00nl1ght.geologicallandforms":
						foreach (Assembly assembly in pack.assemblies.loadedAssemblies)
						{
							if (assembly.GetName().Name == "GeologicalLandforms")
							{
								GeologicalLandformsAssembly = assembly;
								break;
							}
						}

						break;
				}
			}
		}
	}
}