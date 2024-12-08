using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YetAnotherOverlayInterface;

internal class JsonDatabase<T> : IDisposable where T : class, new()
{
	public string Name { get; set; } = string.Empty;
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

	public JsonDatabase(string path, string name, T data = null)
	{
		try {
			Name = name;
			FilePath = path;

			string filePathName = Path.Combine(path, $"{name}.json");
			FileSyncInstance = new(filePathName);
			Load(data);

			JsonWatcherInstance = new(this);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public T Load(T data = null)
	{
		try
		{
			JsonWatcherInstance?.Disable();
			LogManager.Info($"File \"{Name}.json\": Loading...");


			string json = data == null ? FileSyncInstance.Read() : JsonSerializer.Serialize(Data, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);

			LogManager.Info($"File \"{Name}.json\": {json}");

			Data = JsonSerializer.Deserialize<T>(json, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);
			FileSyncInstance.Write(json);


			LogManager.Info($"File \"{Name}.json\": Loaded!");
			JsonWatcherInstance?.DelayedEnable();
			return Data;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			Data = new T();
			Save();
			return Data;
		}
	}

	public bool Save()
	{
		try
		{
			LogManager.Info($"File \"{Name}.json\": Saving...");
			JsonWatcherInstance?.Disable();

			var json = JsonSerializer.Serialize(Data, Constants.JSON_SERIALIZER_OPTIONS_INSTANCE);


			bool isSuccess = FileSyncInstance.Write(json);


			if(isSuccess) LogManager.Info($"File \"{Name}.json\": Saved!");
			else LogManager.Info($"File \"{Name}.json\": Saving failed!");

			JsonWatcherInstance?.DelayedEnable();
			return isSuccess;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			return false;
		}
	}

	public void Delete()
	{
		LogManager.Info($"File \"{Name}.json\": Deleting...");
		Dispose();
		FileSyncInstance.Delete();
		LogManager.Info($"File \"{Name}.json\": Deleted!");
	}

	public void OnChanged()
	{
		Utils.EmitEvents(this, Changed);
	}

	public void OnCreated()
	{
		Utils.EmitEvents(this, Created);
	}

	public void OnRenamedFrom()
	{
		Utils.EmitEvents(this, RenamedFrom);
		Utils.EmitEvents(this, Renamed);
	}

	public void OnRenamedTo()
	{
		Utils.EmitEvents(this, RenamedTo);
		Utils.EmitEvents(this, Renamed);
	}

	public void OnDeleted()
	{
		Utils.EmitEvents(this, Deleted);
	}

	public void Dispose()
	{
		LogManager.Info($"File \"{Name}.json\": Disposing...");
		JsonWatcherInstance?.Dispose();
		LogManager.Info($"File \"{Name}.json\": Disposed!");
	}
}
