using System.Reflection;

namespace PathfindingFramework.ModCompatibility
{
	/// <summary>
	/// Mod compatibility with Vanilla Furniture Expanded - Security.
	/// </summary>
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