using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class CurrentConfigWatcher
{
	private FileSystemWatcher Watcher { get; }

	private bool IsDisabled { get; set; } = false;

	public CurrentConfigWatcher()
	{
		LogManager.Instance.Info("CurrentConfigChangeWatcher: Initializing...");

		Watcher = new(Constants.PLUGIN_DATA_PATH);

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

		Watcher.Filter = $"{Constants.DEFAULT_CONFIG}.json";
		Watcher.EnableRaisingEvents = true;

		LogManager.Instance.Info("CurrentConfigChangeWatcher: Initialized!");
	}

	public void Enable()
	{
		IsDisabled = false;
		LogManager.Instance.Info("CurrentConfigChangeWatcher: Enabled.");
	}

	public void Disable()
	{
		IsDisabled = true;
		LogManager.Instance.Info("CurrentConfigChangeWatcher: Disabled.");
	}

	private void OnCurrentConfigFileChanged(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadCurrentConfig();
	}

	private void OnCurrentConfigFileCreated(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadCurrentConfig();
	}

	private void OnCurrentConfigFileDeleted(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.SaveCurrentConfig();
	}

	private void OnCurrentConfigFileRenamed(object sender, RenamedEventArgs e)
	{
		if(IsDisabled)
		{
			LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.OldFullPath} renamed to {e.FullPath} (Skipped)");
			return;
		}

		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.OldFullPath} renamed to {e.FullPath}");

		ConfigManager.Instance.SaveCurrentConfig();
	}
	
	private void OnCurrentConfigFileError(object sender, ErrorEventArgs e)
	{
		if (IsDisabled) {
			LogManager.Instance.Info($"CurrentConfigChangeWatcher: Error - {e.GetException()} (Skipped)");
			return;
		}
		LogManager.Instance.Error($"CurrentConfigChangeWatcher: Error - {e.GetException()}");

		ConfigManager.Instance.SaveCurrentConfig();
	}
}
