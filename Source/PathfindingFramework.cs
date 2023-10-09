using PathfindingFramework.Cache.Global;
using PathfindingFramework.Parse;
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
			// Caches which require DefDatabase being fully initialized.
			MovementExtensionCache.Initialize();
			// Graphics are initialized after all defs and mod extensions are fully initialized.
			GraphicLoader.Initialize();
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