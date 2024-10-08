using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class JsonWatcher<T>
{
	private JsonDatabase<T> JsonDatabaseInstance { get; }

	private FileSystemWatcher Watcher { get; }

	private string FilePathName { get; } = Constants.PLUGIN_DATA_PATH;

	private bool IsDisabled { get; set; } = false;

	private DateTime LastEventTime { get; set; } = DateTime.MinValue;

	public JsonWatcher(JsonDatabase<T> jsonDatabase)
	{
		JsonDatabaseInstance = jsonDatabase;
		Watcher = new(jsonDatabase.FilePath);
		FilePathName = Path.Combine(jsonDatabase.FilePath, $"{jsonDatabase.Name}.json");

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
	}

	public void Enable()
	{
		IsDisabled = false;
	}

	public void Disable()
	{
		IsDisabled = true;
	}

	private void OnJsonFileChanged(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled) return;

		DateTime eventTime = File.GetLastWriteTime(FilePathName);
		if(LastEventTime.Ticks - eventTime.Ticks < Constants.DUPLICATE_EVENT_TICK_THRESHOLD) return;

		LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\" changed.");

		JsonDatabaseInstance.Load();
		JsonDatabaseInstance.OnChanged();

		LastEventTime = eventTime;
	}

	private void OnJsonFileCreated(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\" created.");

		JsonDatabaseInstance.Load();
		JsonDatabaseInstance.OnCreated();
	}

	private void OnJsonFileDeleted(object sender, FileSystemEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\" deleted.");

		JsonDatabaseInstance.OnDeleted();
	}

	private void OnJsonFileRenamed(object sender, RenamedEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"File \"{e.OldName}\" Renamed to \"{e.Name}\".");

		if(e.Name != Watcher.Filter) JsonDatabaseInstance.OnRenamedFrom();
		else JsonDatabaseInstance.OnRenamedTo();
	}

	private void OnJsonFileError(object sender, ErrorEventArgs e)
	{
		if(IsDisabled) return;
		LogManager.Debug($"File \"{JsonDatabaseInstance.Name}\" error.");

		JsonDatabaseInstance.Load();
	}
}
