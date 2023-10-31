using PathfindingFramework.ModCompatibility;
using PathfindingFramework.MovementDefUtils;
using PathfindingFramework.Parse;
using PathfindingFramework.Patches;
using PathfindingFramework.PawnGraphic;
using PathfindingFramework.PawnMovement;
using PathfindingFramework.RegionGeneration;
using UnityEngine;
using Verse;

namespace PathfindingFramework
{
	/// <summary>
	/// Main class of the mod. Handles initialization order and settings loading. Defines the settings UI.
	/// </summary>
	public class PathfindingFramework : Mod
	{
		public const string PackageId = "pathfinding.framework";
		public const string Name = "Pathfinding Framework";

		/// <summary>
		/// Handles the initialization of every component of this mod.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public PathfindingFramework(ModContentPack content) : base(content)
		{
			ModAssemblyInfo.Initialize();
			Harmony.Initialize();
			ParseHandler.Initialize();
			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		/// <summary>
		/// Initialization steps that must be taken after the game has finished loading.
		/// </summary>
		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			// Reads and stores the MovementDef granted by MovementExtensions of each Def.
			MovementExtensionReader.Initialize();
			// Initialize the extended region types of terrains.
			TerrainRegionType.Initialize();
			// Graphics are initialized after all defs and mod extensions are fully initialized.
			GraphicLoader.Initialize();
			// Enable mod compatibility patches.
			GiddyUp2Compat.Initialize();
			VanillaFurnitureExpandedSecurityCompat.Initialize();
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