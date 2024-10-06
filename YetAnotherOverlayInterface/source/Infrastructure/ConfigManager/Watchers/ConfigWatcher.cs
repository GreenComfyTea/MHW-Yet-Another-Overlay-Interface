using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;
internal class ConfigWatcher
{
	private FileSystemWatcher Watcher { get; }

	private bool IsDisabled { get; set; } = false;

	public ConfigWatcher()
	{
		LogManager.Instance.Info("ConfigChangeWatcher: Initializing...");

		Watcher = new(Constants.CONFIGS_PATH);

		Watcher.NotifyFilter = NotifyFilters.Attributes
							 | NotifyFilters.CreationTime
							 | NotifyFilters.FileName
							 | NotifyFilters.LastWrite
							 | NotifyFilters.Security
							 | NotifyFilters.Size;

		Watcher.Changed += OnCurrentConfigFileChanged;
		Watcher.Created += OnCurrentConfigFileCreated;
		Watcher.Deleted += OnCurrentConfigFileDeleted;
		Watcher.Renamed += OnCurrentConfigFileRenamed;
		Watcher.Error += OnCurrentConfigFileError;

		Watcher.Filter = "*.json";
		Watcher.EnableRaisingEvents = true;

		LogManager.Instance.Info("ConfigChangeWatcher: Initialized!");
	}

	public void Enable()
	{
		IsDisabled = false;
		LogManager.Instance.Info("ConfigChangeWatcher: Enabled.");
	}

	public void Disable()
	{
		IsDisabled = true;
		LogManager.Instance.Info("ConfigChangeWatcher: Disabled.");
	}

	private void OnCurrentConfigFileChanged(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadConfig(e.Name);
	}

	private void OnCurrentConfigFileCreated(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadConfig(e.Name);
	}

	private void OnCurrentConfigFileDeleted(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.RemoveConfig(e.Name);
	}

	private void OnCurrentConfigFileRenamed(object sender, RenamedEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.OldName} renamed to {e.Name} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"ConfigChangeWatcher: File: {e.OldName} renamed to {e.Name}");

		ConfigManager.Instance.RemoveConfig(e.Name);
		ConfigManager.Instance.LoadConfig(e.Name);

		if(ConfigManager.Instance.ActiveConfig.Name == e.OldName)
		{
			ConfigManager.Instance.SetActiveConfig(e.Name);
		}
	}

	private void OnCurrentConfigFileError(object sender, ErrorEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"ConfigChangeWatcher: Error - {e.GetException()} (Skipped)");
			return;
		}

		LogManager.Instance.Error($"ConfigChangeWatcher: Error - {e.GetException()}");
	}
}
