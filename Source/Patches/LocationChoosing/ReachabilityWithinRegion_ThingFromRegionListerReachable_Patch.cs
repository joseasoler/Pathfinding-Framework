using System;
using HarmonyLib;
using PathfindingFramework.MovementContexts;
using Verse;
using Verse.AI;

namespace PathfindingFramework.Patches.LocationChoosing
{
	/// <summary>
	/// Pawns can only reach things in cells they can stand over.
	/// </summary>
	[HarmonyPatch(typeof(ReachabilityWithinRegion), nameof(ReachabilityWithinRegion.ThingFromRegionListerReachable))]
	internal static class ReachabilityWithinRegion_ThingFromRegionListerReachable_Patch
	{
		private static void Postfix(ref bool __result, Thing thing,
			Region region,
			PathEndMode peMode,
			Pawn traveler)
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
				case PathEndMode.Touch:
					if (thing.def.size.x == 1 && thing.def.size.z == 1)
					{
						if (!context.CanStandAt(thing.Position))
						{
							__result = false;
						}
					}
					else
					{
						bool canStandAtAnyCell = false;
						foreach (IntVec3 loc in thing.OccupiedRect())
						{
							if (context.CanStandAt(thing.Position))
							{
								canStandAtAnyCell = true;
								break;
							}
						}

						__result = canStandAtAnyCell;
					}

					break;
				case PathEndMode.InteractionCell:
					if (!context.CanStandAt(thing.InteractionCell))
					{
						__result = false;
					}

					break;
				case PathEndMode.None:
				default:
					break;
			}
		}
	}
}