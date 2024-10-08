using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class JsonDatabase<T>
{
	public string Name { get; set; } = "";
	public string FilePath { get; set; } = Constants.PLUGIN_DATA_PATH;

	public T Data { get; set; }
	public FileSync FileSyncInstance { get; }
	public JsonWatcher<T> JsonWatcherInstance { get; }

	public EventHandler Changed { get; set; } = delegate { };
	public EventHandler Created { get; set; } = delegate { };
	public EventHandler Renamed { get; set; } = delegate { };
	public EventHandler RenamedFrom { get; set; } = delegate { };
	public EventHandler RenamedTo { get; set; } = delegate { };
	public EventHandler Deleted { get; set; } = delegate { };
	public EventHandler Error { get; set; } = delegate { };

	public JsonDatabase(string path, string name)
	{
		try {
			Name = name;
			FilePath = path;

			string filePathName = Path.Combine(path, $"{name}.json");
			FileSyncInstance = new(filePathName);
			Load();

			JsonWatcherInstance = new(this);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	public JsonDatabase(string path, string name, Encoding fileEncoding)
	{
		try
		{
			Name = name;
			FilePath = path;

			string filePathName = Path.Combine(path, $"{name}.json");
			FileSyncInstance = new(filePathName, fileEncoding);
			Load();

			JsonWatcherInstance = new(this);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	public T Load()
	{
		try
		{
			JsonWatcherInstance?.Disable();
			LogManager.Info($"File \"{Name}.json\": Loading...");


			string json = FileSyncInstance.Read();


			Data = JsonSerializer.Deserialize<T>(json, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);

			LogManager.Info($"File \"{Name}.json\": Loaded!");
			Timers.SetTimeout(() => JsonWatcherInstance.Enable(), 50);
			return Data;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
			return Data;
		}
	}

	public bool Save()
	{
		try
		{
			LogManager.Info($"File \"{Name}.json\": Saving...");
			JsonWatcherInstance.Disable();

			var json = JsonSerializer.Serialize(Data, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);


			bool isSuccess = FileSyncInstance.Write(json);


			if(isSuccess) LogManager.Info($"File \"{Name}.json\": Saved!");
			else LogManager.Info($"File \"{Name}.json\": Saving failed!");

			Timers.SetTimeout(() => JsonWatcherInstance.Enable(), 50);
			return isSuccess;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
			return false;
		}
	}

	public void OnChanged()
	{
		EmitEvents(Changed);
	}

	public void OnCreated()
	{
		EmitEvents(Created);
	}

	public void OnRenamedFrom()
	{
		EmitEvents(RenamedFrom);
		EmitEvents(Renamed);
	}

	public void OnRenamedTo()
	{
		EmitEvents(RenamedTo);
		EmitEvents(Renamed);
	}

	public void OnDeleted()
	{
		EmitEvents(Deleted);
	}

	private void EmitEvents(EventHandler eventHandler)
	{
		foreach(Delegate subscriber in eventHandler.GetInvocationList())
		{
			try
			{
				subscriber.DynamicInvoke(this, EventArgs.Empty);
			}

			catch(Exception exception)
			{
				LogManager.Error(exception.Message);
			}
		}
	}
}
