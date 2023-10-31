using System.Reflection;

namespace PathfindingFramework.ModCompatibility
{
	public static class VanillaFurnitureExpandedSecurityCompat
	{
		/// <summary>
		/// Check for mod presence and initialize required data.
		/// </summary>
		public static void Initialize()
		{
			Assembly vanillaFurnitureExpandedSecurityAssembly = ModAssemblyInfo.VanillaFurnitureExpandedSecurity;
			if (vanillaFurnitureExpandedSecurityAssembly == null)
			{
				return;
			}
		}
	}
}