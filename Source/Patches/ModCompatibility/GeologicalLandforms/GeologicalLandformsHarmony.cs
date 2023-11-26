using System;
using System.Reflection;
using PathfindingFramework.ModCompatibility;

namespace PathfindingFramework.Patches.ModCompatibility.GeologicalLandforms
{
	public static class GeologicalLandformsHarmony
	{
		public static void ApplyPatches(HarmonyLib.Harmony harmonyInstance)
		{
			try
			{
				Assembly assemblyInfo = ModAssemblyInfo.GeologicalLandformsAssembly;
				if (assemblyInfo == null)
				{
					return;
				}

				Patch_RimWorld_CellFinder_TryFindRandomExitSpot_Patch.Patch(harmonyInstance);

				Report.Debug("Applied Geological Landforms compatibility patches.");
			}
			catch (Exception exception)
			{
				Report.Error("Geological Landforms harmony compatibility patching failed:");
				Report.Error($"{exception}");
			}
		}
	}
}