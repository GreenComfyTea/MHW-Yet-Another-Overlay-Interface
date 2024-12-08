using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YetAnotherOverlayInterface;

internal class JsonWatcher<T> : IDisposable where T : class, new()
{
	private JsonDatabase<T> JsonDatabaseInstance { get; }

	private FileSystemWatcher Watcher { get; }

	private bool IsDisabled { get; set; } = false;

	private DateTime LastEventTime { get; set; } = DateTime.MinValue;

	public JsonWatcher(JsonDatabase<T> jsonDatabase)
	{
		try
		{
			LogManager.Info($"ConfigWatcher \"{jsonDatabase.Name}\": Initializing...");

			JsonDatabaseInstance = jsonDatabase;
			Watcher = new(jsonDatabase.FilePath);

			Watcher.NotifyFilter = NotifyFilters.Attributes
								 | NotifyFilters.CreationTime
								 | NotifyFilters.FileName
								 | NotifyFilters.LastWrite
								 | NotifyFilters.Security
								 | NotifyFilters.Size;

			Watcher.Changed += OnJsonFileChanged;
			Watcher.Created += OnJsonFileCreated;
			Watcher.Renamed += OnJsonFileRenamed;
			Watcher.Deleted += OnJsonFileDeleted;
			Watcher.Error += OnJsonFileError;

			Watcher.Filter = $"{jsonDatabase.Name}.json";
			Watcher.EnableRaisingEvents = true;

			LogManager.Info($"ConfigWatcher \"{jsonDatabase.Name}\": Initialized!");
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
		LogManager.Info($"ConfigWatcher \"{JsonDatabaseInstance.Name}\": Disposing...");
		Watcher.Dispose();
		LogManager.Info($"ConfigWatcher \"{JsonDatabaseInstance.Name}\": Disposed!");
	}

	private void OnJsonFileChanged(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;

			DateTime eventTime = File.GetLastWriteTime(e.FullPath);
			if(LastEventTime.Ticks - eventTime.Ticks < Constants.DUPLICATE_EVENT_THRESHOLD_TICKS) return;

			LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\": Changed.");

			JsonDatabaseInstance.Load();
			JsonDatabaseInstance.OnChanged();

			LastEventTime = eventTime;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnJsonFileCreated(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;
			LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\": Created.");

			JsonDatabaseInstance.Load();
			JsonDatabaseInstance.OnCreated();
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnJsonFileDeleted(object sender, FileSystemEventArgs e)
	{
		try
		{
			if(IsDisabled) return;
			LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\": Deleted.");

			JsonDatabaseInstance.OnDeleted();
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnJsonFileRenamed(object sender, RenamedEventArgs e)
	{
		try
		{
			if(IsDisabled) return;
			LogManager.Debug($"File \"{e.OldName}\": Renamed to \"{e.Name}\".");

			if(e.Name != Watcher.Filter) JsonDatabaseInstance.OnRenamedFrom();
			else JsonDatabaseInstance.OnRenamedTo();
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnJsonFileError(object sender, ErrorEventArgs e)
	{
		try
		{
			if(IsDisabled) return;
			LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\": Unknown error.");

			JsonDatabaseInstance.Load();
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
		
	}
}
