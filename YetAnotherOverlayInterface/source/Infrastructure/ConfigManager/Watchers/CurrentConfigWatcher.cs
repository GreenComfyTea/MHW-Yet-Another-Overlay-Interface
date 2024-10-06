using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class CurrentConfigWatcher
{
	private FileSystemWatcher Watcher { get; }

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

		LogManager.Instance.Info("CurrentConfigChangeWatcher: Initialization Done!");
	}

	private void OnCurrentConfigFileChanged(object sender, FileSystemEventArgs e)
	{
		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadCurrentConfig();
	}

	private void OnCurrentConfigFileCreated(object sender, FileSystemEventArgs e)
	{
		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.LoadCurrentConfig();
	}

	private void OnCurrentConfigFileDeleted(object sender, FileSystemEventArgs e)
	{
		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.FullPath} {e.ChangeType}");

		ConfigManager.Instance.SaveCurrentConfig();
	}

	private void OnCurrentConfigFileRenamed(object sender, RenamedEventArgs e)
	{
		LogManager.Instance.Info($"CurrentConfigChangeWatcher: File: {e.OldFullPath} renamed to {e.FullPath}");

		ConfigManager.Instance.SaveCurrentConfig();
	}
	
	private void OnCurrentConfigFileError(object sender, ErrorEventArgs e)
	{
		LogManager.Instance.Error($"CurrentConfigChangeWatcher: Error - {e.GetException()}");

		ConfigManager.Instance.SaveCurrentConfig();
	}
}
