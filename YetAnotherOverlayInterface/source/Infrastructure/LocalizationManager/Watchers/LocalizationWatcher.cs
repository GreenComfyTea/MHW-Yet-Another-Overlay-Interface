using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class LocalizationWatcher : IDisposable
{
	private FileSystemWatcher Watcher { get; }

	private bool IsDisabled { get; set; } = false;

	private Dictionary<string, DateTime> LastEventTimes { get; set; } = new();

	public LocalizationWatcher()
	{
		try
		{
			LogManager.Info("LocalizationWatcher: Initializing...");

			Watcher = new(Constants.LOCALIZATIONS_PATH);

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

			LogManager.Info("LocalizationWatcher: Initialized!");
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
		LogManager.Info("LocalizationWatcher: Disposing...");
		Watcher.Dispose();
		LogManager.Info("LocalizationWatcher: Disposed!");
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

			LogManager.Debug($"Localization \"{name}\": Changed.");

			if(LocalizationManager.Instance.Localizations.ContainsKey(name)) return;


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

			LogManager.Debug($"Localization \"{name}\": Created.");

			LocalizationManager.Instance.InitializeLocalization(name);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnConfigFileDeleted(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;

			string name = Path.GetFileNameWithoutExtension(e.Name);

			LogManager.Debug($"Localization \"{name}\": Deleted.");
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

			LogManager.Debug($"Localization \"{oldName}\": Renamed to \"{name}\".");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnConfigFileError(object sender, ErrorEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"LocalizationWatcher: Unknown error.");
	}
}
