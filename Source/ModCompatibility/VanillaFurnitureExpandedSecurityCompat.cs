using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace PathfindingFramework.ModCompatibility
{
	using FinalTerrainPathCostDelegate = Func<int, TerrainDef, IntVec3, Map, int>;

	/// <summary>
	/// Mod compatibility with Vanilla Furniture Expanded - Security.
	/// </summary>
	public static class VanillaFurnitureExpandedSecurityCompat
	{
		private static FinalTerrainPathCostDelegate _finalTerrainPathCost;

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

			MethodInfo finalTerrainPathCostInfo =
				ModCompatibilityUtility.MethodFromAssembly(vanillaFurnitureExpandedSecurityAssembly, "CalculatedCostAt",
					"FinalTerrainPathCost");

			_finalTerrainPathCost = AccessTools.MethodDelegate<FinalTerrainPathCostDelegate>(finalTerrainPathCostInfo);
		}

		public static int MoveIntoCellTerrainCost(int originalPathCost, TerrainDef currentTerrainDef, IntVec3 prevCell,
			Map map)
		{
			return _finalTerrainPathCost?.Invoke(originalPathCost, currentTerrainDef, prevCell, map) ?? originalPathCost;
		}
	}
}