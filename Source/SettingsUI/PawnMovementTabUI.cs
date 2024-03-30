using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	using PawnMovementEntry = Tuple<PawnKindDef, Color, Texture>;

	public class PawnMovementTabUI
	{
		/// <summary>
		/// Used to highlight the pawn movement row currently under the mouse.
		/// </summary>
		private static readonly Color HighlightColor = new(1.0F, 1.0F, 1.0F, 0.5f);

		private const float AnimalEntryHeight = 48F;

		/// <summary>
		/// Current scroll position of the PawnMovement tab.
		/// </summary>
		private static Vector2 _pawnMovementScrollPosition;

		/// <summary>
		/// List of entries of the pawn movement window.
		/// </summary>
		private List<PawnMovementEntry> _pawnMovementEntries;

		/// <summary>
		/// Can filter animals by label, defName, or packageId.
		/// </summary>
		private string _filterText;

		/// <summary>
		/// Checks if a pawnKindDef should appear on the pawn movement settings tab.
		/// </summary>
		/// <param name="pawnKindDef">Pawn kind to check.</param>
		/// <param name="ignoreDuplicates">Used to ignore pawnKindDefs sharing the same pawnKindDef.race.</param>
		/// <returns>True if this should be a valid entry.</returns>
		private static bool IsValidPawnKindDef(PawnKindDef pawnKindDef, HashSet<ThingDef> ignoreDuplicates)
		{
			const string vehiclesFrameworkPawnKindDef = "VehicleDef";

			return
				// Ignore invalid entries.
				pawnKindDef.race?.race != null &&
				// Do not show vehicles from Vehicles Framework.
				pawnKindDef.race.GetType().Name != vehiclesFrameworkPawnKindDef &&
				// Ignore duplicates.
				!ignoreDuplicates.Contains(pawnKindDef.race);
		}

		/// <summary>
		/// Generate pawn movement entry data from a pawnKindDef.
		/// </summary>
		/// <param name="pawnKindDef">Pawn kind being generated.</param>
		/// <returns>Data required to show this entry on the settings tab.</returns>
		private static PawnMovementEntry GetPawnMovementEntry(PawnKindDef pawnKindDef)
		{
			Color color = Color.white;
			Texture texture = null;
			if (pawnKindDef.lifeStages != null)
			{
				int lastLifeStageIndex = pawnKindDef.lifeStages.Count - 1;
				if (lastLifeStageIndex >= 0)
				{
					PawnKindLifeStage lastLifeStage = pawnKindDef.lifeStages[lastLifeStageIndex];
					// See ThingDef.ResolveIcon()
					Material material = lastLifeStage.bodyGraphicData?.Graphic?.MatEast;
					if (material != null)
					{
						color = material.color;
						texture = material.mainTexture;
					}
				}
			}

			return new PawnMovementEntry(pawnKindDef, color, texture);
		}

		/// <summary>
		/// Lazy initialization of pawn movement entries.
		/// </summary>
		private void InitializePawnMovementEntries()
		{
			if (_pawnMovementEntries != null)
			{
				return;
			}

			_pawnMovementEntries = new List<PawnMovementEntry>();
			HashSet<ThingDef> ignoreDuplicates = new HashSet<ThingDef>();

			List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
			for (int pawnIndex = 0; pawnIndex < pawnKindDefs.Count; ++pawnIndex)
			{
				PawnKindDef pawnKindDef = pawnKindDefs[pawnIndex];
				try
				{
					if (!IsValidPawnKindDef(pawnKindDef, ignoreDuplicates))
					{
						continue;
					}

					ignoreDuplicates.Add(pawnKindDef.race);
					_pawnMovementEntries.Add(GetPawnMovementEntry(pawnKindDef));
				}
				catch (Exception exception)
				{
					Report.Error($"Pawn movement settings failed to initialize PawnKindDef {pawnKindDef}:");
					Report.Error($"{exception}");
				}
			}

			_pawnMovementEntries.Sort(new ComparePawnMovementEntries());
		}

		/// <summary>
		/// Helper function to save the new movement type override for a pawn.
		/// </summary>
		/// <param name="raceDef">Race of the pawnKindDef.</param>
		/// <param name="movementDefName">String name of the movementDef.</param>
		private static void SavePawnMovementDef(ThingDef raceDef, string movementDefName)
		{
			Settings.Values.PawnMovementOverrides[raceDef.defName] = movementDefName;
		}

		/// <summary>
		/// Generate the dropdown of valid movement types to choose for a pawn.
		/// </summary>
		/// <param name="raceDef">Race of the pawnKindDef.</param>
		/// <returns>Collection of dropdown menu entries.</returns>
		private static IEnumerable<Widgets.DropdownMenuElement<string>> GenerateMovementDefMenu(ThingDef raceDef)
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			movementDefs.Sort((lhs, rhs) => lhs.priority.CompareTo(rhs.priority));

			for (int movementDefIndex = 0; movementDefIndex < movementDefs.Count; ++movementDefIndex)
			{
				MovementDef movementDef = movementDefs[movementDefIndex];
				if (
					// Avoid showing this internal MovementDef to players.
					movementDef == MovementDefOf.PF_Movement_Terrestrial_Unsafe ||
					// Human-like pawns should not use movement types which fully prevent access to passable tiles.
					// At the moment this recommendation is not enforced for custom movement types.
					raceDef.race.Humanlike && movementDef == MovementDefOf.PF_Movement_Aquatic)
				{
					continue;
				}

				string movementDefName = movementDef.defName;
				yield return new Widgets.DropdownMenuElement<string>()
				{
					option = new FloatMenuOption(movementDef.LabelCap, () => SavePawnMovementDef(raceDef, movementDefName)),
					payload = movementDefName
				};
			}
		}

		/// <summary>
		/// Draw the movement change row for one pawn.
		/// </summary>
		/// <param name="pawnKindDef">Pawn to draw.</param>
		/// <param name="color">Recolor to use for the texture of this pawn.</param>
		/// <param name="texture">Texture to draw.</param>
		/// <param name="rowRect">Available area.</param>
		private static void DrawPawnMovementRow(PawnKindDef pawnKindDef, Color color, Texture texture, Rect rowRect)
		{
			// First cell: textures use a square space.
			Rect textureRect = new Rect(rowRect.x, rowRect.y, rowRect.height, rowRect.height);

			// Last cell: the dropdown has the standard button width size.
			Rect dropdownRect = new Rect(rowRect.x + rowRect.width - Window.CloseButSize.x, rowRect.y, Window.CloseButSize.x,
				rowRect.height);

			// The remaining width is shared between the last two cells.
			float remainingIntervalStart = textureRect.x + textureRect.width;
			float remainingWidth = dropdownRect.x - remainingIntervalStart;

			// Second cell: label
			Rect labelRect = new Rect(remainingIntervalStart, rowRect.y, 2.0F * remainingWidth / 5.0F, rowRect.height);
			// Third cell: mod
			Rect modRect = new Rect(labelRect.x + labelRect.width, rowRect.y, 3.0F * remainingWidth / 5.0F, rowRect.height);

			if (texture != null)
			{
				GUI.color = color;
				GUI.DrawTexture(textureRect, texture);
				GUI.color = Color.white;
			}

			Widgets.Label(labelRect, pawnKindDef.race.LabelCap);


			Widgets.Label(modRect, ModOf(pawnKindDef));

			MovementDef movementDef = PawnMovementOverrideSettings.CurrentMovementDef(pawnKindDef.race) ??
			                          MovementDefOf.PF_Movement_Terrestrial;
			Widgets.Dropdown(dropdownRect, pawnKindDef.race, _ => movementDef.defName, GenerateMovementDefMenu,
				movementDef.LabelCap.ToString());

			if (Mouse.IsOver(rowRect))
			{
				GUI.DrawTexture(rowRect, TexUI.HighlightSelectedTex);
			}
		}

		private static string ModOf(PawnKindDef pawnKindDef)
		{
			return pawnKindDef.modContentPack != null
				? pawnKindDef.modContentPack.Name
				: Translations.Unknown;
		}

		/// <summary>
		/// Helper function for a case-insensitive comparison of an arbitrary string against the current filter.
		/// </summary>
		/// <param name="str">String to check.</param>
		/// <returns>True if the string is a match.</returns>
		private bool FilterMatch(string str)
		{
			return str.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private List<PawnMovementEntry> FilterEntries()
		{
			if (_filterText == "")
			{
				return _pawnMovementEntries;
			}

			List<PawnMovementEntry> filteredEntries = new List<PawnMovementEntry>();
			foreach (PawnMovementEntry entry in _pawnMovementEntries)
			{
				PawnKindDef pawnKindDef = entry.Item1;
				if (FilterMatch(pawnKindDef.race.label) || FilterMatch(pawnKindDef.defName) || FilterMatch(ModOf(pawnKindDef)))
				{
					filteredEntries.Add(entry);
				}
			}

			return filteredEntries;
		}

		/// <summary>
		/// Draw the contents of the pawn movement tab.
		/// When in-game, it will just draw a warning. This is because the mod does not support regenerating the movement
		/// contexts of existing pawns to change their movement type. Instead, the user is forced to reload so that
		/// the movement contexts can regenerate with the new settings.
		/// </summary>
		/// <param name="inRect">Available draw area.</param>
		public void Contents(Rect inRect)
		{
			const float animalEntryGap = GenUI.GapTiny;
			TextAnchor anchorBackup = Text.Anchor;
			if (Current.ProgramState != ProgramState.Entry)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(inRect, Translations.PF_PawnMovementWarningLabel);
				Text.Anchor = anchorBackup;
				return;
			}

			InitializePawnMovementEntries();
			Rect filterRect = inRect.TopPartPixels(GenUI.GapWide);
			_filterText = Widgets.TextField(filterRect, _filterText).ToLower();

			inRect = inRect.BottomPartPixels(inRect.height - GenUI.GapWide - GenUI.GapSmall);
			Rect outRect = inRect.ContractedBy(GenUI.GapSmall);

			List<PawnMovementEntry> filteredEntries = FilterEntries();
			int entryCount = filteredEntries.Count;

			Rect viewRect = new Rect(0, 0, outRect.width - GenUI.ScrollBarWidth,
				(AnimalEntryHeight + animalEntryGap) * entryCount);
			Widgets.BeginScrollView(inRect, ref _pawnMovementScrollPosition, viewRect);

			Listing_Standard listing = new Listing_Standard();
			listing.Begin(viewRect);

			Text.Anchor = TextAnchor.MiddleCenter;

			float minHeightThreshold = _pawnMovementScrollPosition.y - AnimalEntryHeight;
			float maxHeightThreshold = _pawnMovementScrollPosition.y + viewRect.height;
			foreach (var (pawnKindDef, color, texture) in filteredEntries)
			{
				Rect entryRect = listing.GetRect(AnimalEntryHeight);
				float currentHeight = entryRect.y;
				// Entries are only processed if they are inside of the viewable area.
				if (currentHeight > minHeightThreshold && currentHeight < maxHeightThreshold)
				{
					try
					{
						DrawPawnMovementRow(pawnKindDef, color, texture, entryRect);
					}
					catch (Exception exception)
					{
						Report.ErrorOnce($"Pawn movement settings failed to draw row for PawnKindDef {pawnKindDef}: {exception}");
					}
				}

				listing.Gap(animalEntryGap);
			}

			Text.Anchor = anchorBackup;

			listing.End();
			Widgets.EndScrollView();
		}
	}
}