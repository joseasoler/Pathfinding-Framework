using System;
using System.Reflection;
using Verse;

namespace PathfindingFramework.ModCompatibility
{
	public static class WindowsCompat
	{
		private static Type BuildingWindowType = null;

		/// <summary>
		/// Check for mod presence and initialize required data.
		/// </summary>
		public static void Initialize()
		{
			Assembly windowsAssembly = ModAssemblyInfo.Windows;
			if (windowsAssembly == null)
			{
				return;
			}

			BuildingWindowType = ModCompatibilityUtility.TypeFromAssembly(windowsAssembly, "Building_Window");
		}

		public static bool IsWindow(Thing thing)
		{
			return thing.GetType() == BuildingWindowType;
		}
	}
}