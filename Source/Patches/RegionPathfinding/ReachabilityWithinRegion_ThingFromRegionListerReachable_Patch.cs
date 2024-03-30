using HarmonyLib;
using PathfindingFramework.ExtensionMethodCaches;
using PathfindingFramework.MapComponents.MovementContexts;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.RegionPathfinding
{
	/// <summary>
	/// Pawns can only reach things in cells they can stand over.
	/// </summary>
	[HarmonyPatch(typeof(ReachabilityWithinRegion), nameof(ReachabilityWithinRegion.ThingFromRegionListerReachable))]
	public static class ReachabilityWithinRegion_ThingFromRegionListerReachable_Patch
	{
		public static void Postfix(ref bool __result, Thing thing, PathEndMode peMode, Pawn traveler)
		{
			if (!__result || traveler == null)
			{
				return;
			}

			MovementContext context = traveler.MovementContext();
			switch (peMode)
			{
				case PathEndMode.ClosestTouch:
				case PathEndMode.OnCell:
					if (thing.def.size.x == 1 && thing.def.size.z == 1)
					{
						if (!context.CanEnterTerrain(thing.Position))
						{
							__result = false;
						}
					}
					else
					{
						bool canEnterAnyCell = false;
						foreach (IntVec3 loc in thing.OccupiedRect())
						{
							if (context.CanEnterTerrain(loc))
							{
								canEnterAnyCell = true;
								break;
							}
						}

						__result = canEnterAnyCell;
					}

					break;

				case PathEndMode.InteractionCell:
					if (!context.CanEnterTerrain(thing.InteractionCell))
					{
						__result = false;
					}

					break;

				case PathEndMode.Touch:
					if (context.CanEnterTerrain(thing.Position))
					{
						__result = true;
					}
					else
					{
						bool canEnterAnyAdjacentCell = false;
						foreach (IntVec3 loc in GenAdj.CellsAdjacent8Way(thing))
						{
							if (context.CanEnterTerrain(loc))
							{
								canEnterAnyAdjacentCell = true;
								break;
							}
						}

						__result = canEnterAnyAdjacentCell;
					}

					break;

				case PathEndMode.None:
				default:
					break;
			}
		}
	}
}