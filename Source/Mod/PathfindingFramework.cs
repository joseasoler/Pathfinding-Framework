using System;
using System.Collections.Generic;
using PathfindingFramework.Def;
using PathfindingFramework.HarmonyPatches;
using PathfindingFramework.MovementTypes;
using UnityEngine;
using Verse;

namespace PathfindingFramework.Mod
{
	public class PathfindingFramework : Verse.Mod
	{
		public const string PackageId = "pathfinding.framework";

		/// <summary>
		/// Handles the initialization of every component of this mod.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public PathfindingFramework(ModContentPack content) : base(content)
		{
			HarmonyHandler.Initialize();
			ParseHandler.Initialize();
			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		/// <summary>
		/// Initialization steps that must be taken after the game has finished loading.
		/// </summary>
		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			MovementTypesHandler.Initialize();
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return Report.Name;
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("PF_DebugLogLabel".Translate(), ref Settings.Values.DebugLog,
				"PF_DebugLogHover".Translate());
			

			listing.Gap();
			var buttonsRect = listing.GetRect(30.0F);
			var buttonWidth = buttonsRect.width / 5.0F;

			var movementTypeReportRect = new Rect(buttonsRect.x, buttonsRect.y, buttonWidth, buttonsRect.height);
			if (Widgets.ButtonText(movementTypeReportRect, "PF_MovementTypesReportLabel".Translate()))
			{
				MovementTypesHandler.ShowReport();
			}
			TooltipHandler.TipRegion(movementTypeReportRect, "PF_MovementTypesReportHover".Translate());

			var resetRect = new Rect(buttonsRect.width - buttonWidth, buttonsRect.y, buttonWidth, buttonsRect.height);
			if (Widgets.ButtonText(resetRect, "PF_ResetSettingsLabel".Translate()))
			{
				Settings.Reset();
			}
			TooltipHandler.TipRegion(resetRect, "PF_ResetSettingsHover".Translate());


			listing.End();
			base.DoSettingsWindowContents(inRect);
		}
	}
}