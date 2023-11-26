using System.Collections.Generic;
using PathfindingFramework.ModCompatibility;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.Parse;
using PathfindingFramework.PawnGraphic;
using PathfindingFramework.SettingsUI;
using PathfindingFramework.TerrainDefInformation;
using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Main class of the mod. Handles initialization order and settings loading. Defines the settings UI.
	/// </summary>
	public class PathfindingFrameworkMod : Mod
	{
		public const string PackageId = "pathfinding.framework";
		public const string Name = "Pathfinding Framework";

		/// <summary>
		/// Handles the initialization of every component of this mod.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public PathfindingFrameworkMod(ModContentPack content) : base(content)
		{
			// Parsers are initialized even if prepatcher is missing to load Defs and Extensions without errors.
			ParseHandler.Initialize();
			ModAssemblyInfo.Initialize();
			if (!ModAssemblyInfo.PrepatcherPresent)
			{
				Report.Error(
					$"Pathfinding Framework requires Prepatcher, but this mod was not found in the modlist. Pathfinding Framework will be disabled.");
				return;
			}

			Harmony.Initialize();
			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		/// <summary>
		/// Initialization steps that must be taken after the game has finished loading.
		/// </summary>
		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			// Certain mods such as Geological Landforms dynamically load additional DLLs at a different game load stage.
			ModAssemblyInfo.LoadingFinished();
			// Additional Harmony patching that must wait until this stage, due to a dependency with ModAssemblyInfo.LoadingFinished.
			Harmony.LoadingFinished();
			// This value might be null after loading a config which lacks it.
			Settings.Values.PawnMovementOverrides ??= new Dictionary<string, string>();
			// Reads and stores the MovementDef granted by MovementExtensions of each Def.
			MovementExtensionReader.Initialize();
			// Apply movement type overrides from settings. Must be initialized after GetSettings and MovementExtensionReader.
			PawnMovementOverrideSettings.Initialize();
			// Set the indexes of TerrainDefs to use when accessing the path cost arrays of MovementDefs.
			TerrainMovementIndex.Initialize();
			// Precalculate movement path costs for each terrain.
			MovementDefPathCosts.Initialize();
			// Initialize the extended region types of terrains.
			TerrainRegionType.Initialize();
			// Graphics are initialized after all defs and mod extensions are fully initialized.
			GraphicLoader.Initialize();
			// Enable mod compatibility patches.
			GiddyUp2Compat.Initialize();
			VanillaFurnitureExpandedSecurityCompat.Initialize();
			WindowsCompat.Initialize();
			// Settings window initialization.
			SettingsWindow.Initialize(WriteSettings);
			// Add some useful (but brief) information to the log.
			// Enabling debug logging expands upon this.
			LoadedDataReport.Write();
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return SettingsWindow.SettingsCategory();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			SettingsWindow.DoWindowContents(inRect);
			base.DoSettingsWindowContents(inRect);
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			SettingsWindow.OnWriteSettings();
		}
	}
}