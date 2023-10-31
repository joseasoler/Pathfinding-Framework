using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PathfindingFramework.PawnGraphic
{
	/// <summary>
	/// This class collates all graphic changes coming from mod extensions, keeps track of the conditions of each
	/// potential graphic change, and applies them when necessary.
	/// At the moment the type is really simple, but the feature is already encapsulated here to simplify future
	/// extensions such as life stage changes.
	/// </summary>
	public class GraphicContext
	{
		/// <summary>
		/// Owner of this context.
		/// </summary>
		private readonly Pawn _pawn;

		/// <summary>
		/// Graphical changes triggered by locomotion changes.
		/// </summary>
		private readonly LocomotionGraphicExtension _locomotionGraphic;

		/// <summary>
		/// Graphical changes triggered by current terrain changes.
		/// </summary>
		private readonly TerrainTagGraphicExtension _terrainTagGraphic;

		private readonly int _alternateGraphicIndex;

		public GraphicContext(Pawn pawn)
		{
			_pawn = pawn;
			_locomotionGraphic = pawn.kindDef.GetModExtension<LocomotionGraphicExtension>();
			_terrainTagGraphic = pawn.kindDef.GetModExtension<TerrainTagGraphicExtension>();
			if (_locomotionGraphic == null && _terrainTagGraphic == null)
			{
				Report.Error($"Pawn {_pawn.GetUniqueLoadID()} initialized a GraphicContext without any graphic extensions.");
			}

			_alternateGraphicIndex = int.MinValue;
			if (pawn.kindDef.alternateGraphics.NullOrEmpty())
			{
				return;
			}

			// Replicate the random alternate graphic logic in PawnGraphicSet.ResolveAllGraphics().
			Rand.PushState(_pawn.thingIDNumber ^ 46101);
			if (Rand.Value <= _pawn.kindDef.alternateGraphicChance)
			{
				List<Pair<int, float>> indexesByWeight = new List<Pair<int, float>>();
				for (int alternateIndex = 0; alternateIndex < _pawn.kindDef.alternateGraphics.Count; ++alternateIndex)
				{
					indexesByWeight.Add(new Pair<int, float>(alternateIndex,
						_pawn.kindDef.alternateGraphics[alternateIndex].Weight));
				}

				_alternateGraphicIndex = indexesByWeight.RandomElementByWeight(x => x.Second).First;
			}

			Rand.PopState();
		}

		private bool Active()
		{
			return _pawn.Spawned && !_pawn.Dead;
		}

		private void RecalculatePawnGraphicSet()
		{
			if (Active())
			{
				_pawn.drawer.renderer.graphics.nakedGraphic = null;
			}
		}

		/// <summary>
		/// Called when the locomotion urgency of the pawn has changed.
		/// </summary>
		public void LocomotionUpdated()
		{
			if (_locomotionGraphic != null)
			{
				RecalculatePawnGraphicSet();
			}
		}

		/// <summary>
		/// Called when the pawn moves into a new terrain type.
		/// </summary>
		/// <param name="previousTerrainDef">Terrain type of the previous cell of the pawn.</param>
		/// <param name="currentTerrainDef">Terrain type of the new cell of the pawn.</param>
		public void TerrainUpdated(TerrainDef previousTerrainDef, TerrainDef currentTerrainDef)
		{
			if (_terrainTagGraphic != null && _terrainTagGraphic.Affects(previousTerrainDef) !=
			    _terrainTagGraphic.Affects(currentTerrainDef))
			{
				RecalculatePawnGraphicSet();
			}
		}

		/// <summary>
		/// Obtains the replacement graphic.
		/// </summary>
		/// <param name="nakedGraphic">Naked graphic of the pawn without modifications.</param>
		/// <param name="extension">Extensions which needs to modify the graphics.</param>
		/// <returns>New graphic to use.</returns>
		private Graphic GetExtensionGraphic(Graphic nakedGraphic, GraphicExtension extension)
		{
			// nakedGraphic has already been checked for null value prior to this call.

			Graphic graphic = extension.bodyGraphicData.Graphic;
			// Alternate graphics are only processed if both lists have the same size.
			// Validation of this has already been done at GraphicLoader.
			if (_alternateGraphicIndex >= 0 && _pawn.kindDef.alternateGraphics != null &&
			    extension.alternateGraphics != null &&
			    _pawn.kindDef.alternateGraphics.Count == extension.alternateGraphics.Count)
			{
				graphic = extension.alternateGraphics[_alternateGraphicIndex].Graphic;
			}

			// Return a copy matching the drawSize and shader of the original.
			return graphic?.GetCopy(nakedGraphic.drawSize, nakedGraphic.Shader);
		}

		private bool TryApplyLocomotionChanges(PawnGraphicSet graphicSet)
		{
			if (_locomotionGraphic == null || !_pawn.pather.Moving)
			{
				// Locomotion graphic changes are only applied if the pawn is moving.
				return false;
			}

			LocomotionUrgency locomotion = CurrentUrgencyUtil.Get(_pawn);

			if (!_locomotionGraphic.locomotionUrgencies.Contains(locomotion))
			{
				return false;
			}

			graphicSet.nakedGraphic = GetExtensionGraphic(graphicSet.nakedGraphic, _locomotionGraphic);

			return true;
		}

		private void TryApplyTerrainTagChanges(PawnGraphicSet graphicSet)
		{
			if (_terrainTagGraphic == null || !_terrainTagGraphic.Affects(_pawn.Position.GetTerrain(_pawn.Map)))
			{
				return;
			}

			graphicSet.nakedGraphic = GetExtensionGraphic(graphicSet.nakedGraphic, _terrainTagGraphic);
		}

		public void ApplyGraphicChanges(PawnGraphicSet graphicSet)
		{
			if (!Active() || graphicSet.nakedGraphic == null)
			{
				return;
			}

			if (!TryApplyLocomotionChanges(graphicSet))
			{
				TryApplyTerrainTagChanges(graphicSet);
			}
		}
	}
}