using System;
using System.Collections.Generic;
using PathfindingFramework.Patches;
using UnityEngine;
using Verse;

namespace PathfindingFramework.SettingsUI
{
	using PawnMovementEntry = Tuple<PawnKindDef, Texture>;

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

		private static bool IsValidPawnKindDef(PawnKindDef pawnKindDef, HashSet<ThingDef> ignoreDuplicates)
		{
			// Avoid showing Vehicles Framework pawns.
			const string vehiclesFrameworkPawnKindDef = "VehicleDef";

			return pawnKindDef.race?.race != null &&
				pawnKindDef.race.GetType().Name != vehiclesFrameworkPawnKindDef &&
				!ignoreDuplicates.Contains(pawnKindDef.race);
		}

		private static Texture GetTextureFromPawnKindDef(PawnKindDef pawnKindDef)
		{
			if (pawnKindDef.lifeStages == null)
			{
				return null;
			}

			int lastLifeStageIndex = pawnKindDef.lifeStages.Count - 1;
			if (lastLifeStageIndex < 0)
			{
				return null;
			}

			PawnKindLifeStage lastLifeStage = pawnKindDef.lifeStages[lastLifeStageIndex];
			return lastLifeStage.bodyGraphicData?.Graphic?.MatSingle.mainTexture;
		}

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
				if (!IsValidPawnKindDef(pawnKindDef, ignoreDuplicates))
				{
					continue;
				}

				ignoreDuplicates.Add(pawnKindDef.race);
				_pawnMovementEntries.Add(new PawnMovementEntry(pawnKindDef, GetTextureFromPawnKindDef(pawnKindDef)));
			}

			_pawnMovementEntries.Sort(new ComparePawnMovementEntries());
		}

		private static void SavePawnMovementDef(string raceDefName, string movementDefName)
		{
			Settings.Values.PawnMovementOverrides[raceDefName] = movementDefName;
		}

		private static IEnumerable<Widgets.DropdownMenuElement<string>> GenerateMovementDefMenu(string raceDefName)
		{
			List<MovementDef> movementDefs = DefDatabase<MovementDef>.AllDefsListForReading;
			movementDefs.Sort((lhs, rhs) => lhs.priority.CompareTo(rhs.priority));

			for (int movementDefIndex = 0; movementDefIndex < movementDefs.Count; ++movementDefIndex)
			{
				MovementDef movementDef = movementDefs[movementDefIndex];
				if (movementDef == MovementDefOf.PF_Movement_Terrestrial_Unsafe)
				{
					continue;
				}

				string movementDefName = movementDef.defName;

				yield return new Widgets.DropdownMenuElement<string>()
				{
					option = new FloatMenuOption(movementDef.LabelCap, () => SavePawnMovementDef(raceDefName, movementDefName)),
					payload = movementDefName
				};
			}
		}

		private static void AnimalEntry(PawnKindDef pawnKindDef, Texture texture, Rect rowRect)
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
				GUI.DrawTexture(textureRect, texture);
			}

			Widgets.Label(labelRect, pawnKindDef.race.LabelCap);

			string mod = pawnKindDef.modContentPack != null
				? pawnKindDef.modContentPack.Name
				: "Unknown".Translate().ToString();
			Widgets.Label(modRect, mod);

			MovementDef movementDef = PawnMovementOverrideSettings.CurrentMovementDef(pawnKindDef.race);
			Widgets.Dropdown(dropdownRect, pawnKindDef.race.defName, _ => movementDef.defName, GenerateMovementDefMenu,
				movementDef.LabelCap.ToString());

			if (Mouse.IsOver(rowRect))
			{
				GUI.DrawTexture(rowRect, TexUI.HighlightSelectedTex);
			}
		}

		public void Contents(Rect inRect)
		{
			const float animalEntryGap = GenUI.GapTiny;
			InitializePawnMovementEntries();

			Rect outRect = inRect.ContractedBy(GenUI.GapSmall);

			int entryCount = _pawnMovementEntries.Count;

			Rect viewRect = new Rect(0, 0, outRect.width - GenUI.ScrollBarWidth,
				GenUI.GapWide + GenUI.GapSmall + AnimalEntryHeight * entryCount + animalEntryGap * entryCount);
			Widgets.BeginScrollView(inRect, ref _pawnMovementScrollPosition, viewRect);

			Listing_Standard listing = new Listing_Standard();
			listing.Begin(viewRect);


			TextAnchor anchorBackup = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleCenter;

			listing.Label("PF_PawnMovementWarningLabel".Translate(), GenUI.GapWide);
			listing.Gap(GenUI.GapSmall);

			bool first = true;
			foreach (var (raceDef, texture) in _pawnMovementEntries)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					listing.Gap(animalEntryGap);
				}

				AnimalEntry(raceDef, texture, listing.GetRect(AnimalEntryHeight));
			}

			Text.Anchor = anchorBackup;

			listing.End();
			Widgets.EndScrollView();
		}
	}
}