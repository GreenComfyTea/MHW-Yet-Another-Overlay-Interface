using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class ConfigWatcher : IDisposable
{
	private FileSystemWatcher Watcher { get; }

	private bool IsDisabled { get; set; } = false;

	private Dictionary<string, DateTime> LastEventTimes { get; set; } = new();

	public ConfigWatcher()
	{
		try
		{
			LogManager.Info("ConfigWatcher: Initializing...");

			Watcher = new(Constants.CONFIGS_PATH);

			Watcher.NotifyFilter = NotifyFilters.Attributes
								 | NotifyFilters.CreationTime
								 | NotifyFilters.FileName
								 | NotifyFilters.LastWrite
								 | NotifyFilters.Security
								 | NotifyFilters.Size;

			Watcher.Changed += OnConfigFileChanged;
			Watcher.Created += OnConfigFileCreated;
			Watcher.Renamed += OnConfigFileRenamed;
			Watcher.Deleted += OnConfigFileDeleted;
			Watcher.Error += OnConfigFileError;

			Watcher.Filter = "*.json";
			Watcher.EnableRaisingEvents = true;

			LogManager.Info("ConfigWatcher: Initialized!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public void Enable()
	{
		IsDisabled = false;
	}

	public void DelayedEnable()
	{
		Timers.SetTimeout(Enable, Constants.REENABLE_WATCHER_DELAY_MILLISECONDS);
	}

	public void Disable()
	{
		IsDisabled = true;
	}

	public void Dispose()
	{
		LogManager.Info("ConfigWatcher: Disposing...");
		Watcher.Dispose();
		LogManager.Info("ConfigWatcher: Disposed!");
	}

	private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;

			string name = Path.GetFileNameWithoutExtension(e.Name);

			DateTime eventTime = File.GetLastWriteTime(e.FullPath);
			if(!LastEventTimes.ContainsKey(name)) LastEventTimes[name] = DateTime.MinValue;

			if(LastEventTimes[name].Ticks - eventTime.Ticks < Constants.DUPLICATE_EVENT_THRESHOLD_TICKS) return;

			LogManager.Debug($"Config \"{name}\": Changed.");

			if(ConfigManager.Instance.Configs.ContainsKey(name)) return;

			LastEventTimes[name] = eventTime;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}

		
	}

	private void OnConfigFileCreated(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;

			string name = Path.GetFileNameWithoutExtension(e.Name);

			LogManager.Debug($"Config \"{name}\": Created.");

			ConfigManager.Instance.InitializeConfig(name);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnConfigFileDeleted(object sender, FileSystemEventArgs e)
	{
		try {
			if(IsDisabled) return;

			string name = Path.GetFileNameWithoutExtension(e.Name);

			LogManager.Debug($"Config \"{name}\": Deleted.");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
		
	}

	private void OnConfigFileRenamed(object sender, RenamedEventArgs e)
	{
		try
		{
			if(IsDisabled) return;

			string oldName = Path.GetFileNameWithoutExtension(e.OldName);
			string name = Path.GetFileNameWithoutExtension(e.Name);

			LogManager.Debug($"Config \"{oldName}\": Renamed to \"{name}\".");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnConfigFileError(object sender, ErrorEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"ConfigWatcher: Unknown error.");
	}
}
