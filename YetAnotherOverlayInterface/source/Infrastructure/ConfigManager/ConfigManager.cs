using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YetAnotherOverlayInterface;

internal class ConfigManager : IDisposable
{
	private static readonly Lazy<ConfigManager> _lazy = new(() => new ConfigManager());
	public static ConfigManager Instance => _lazy.Value;

	public ConfigCustomization Customization { get; set; }

	public JsonDatabase<Config> ActiveConfig { get; set; }
	public Dictionary<string, JsonDatabase<Config>> Configs { get; set; } = [];

	public EventHandler ActiveConfigChanged { get; set; } = delegate { };

	public ConfigWatcher ConfigWatcherInstance { get; set; }

	private JsonDatabase<CurrentConfig> CurrentConfigInstance { get; set; }

	private ConfigManager() {}

	public void Initialize()
	{
		LogManager.Info("ConfigManager: Initializing...");

		LoadAllConfigs();
		LoadCurrentConfig();

		ConfigWatcherInstance = new();
		Customization = new();

		LogManager.Info("ConfigManager: Initialized!");
	}

	public void ActivateConfig(JsonDatabase<Config> config)
	{
		LogManager.Info($"ConfigManager: Activating config \"{config.Name}\"...");
		
		ActiveConfig = config;
		CurrentConfigInstance.Data.ConfigName = config.Name;
		CurrentConfigInstance.Save();

		OnActiveConfigChanged();

		LogManager.Info($"ConfigManager: Config \"{config.Name}\" is activated!");
	}

	public void ActivateConfig(string name)
	{
		LogManager.Info($"ConfigManager: Searching for config \"{name}\" to activate it...");

		bool isGetConfigSuccess = Configs.TryGetValue(name, out JsonDatabase<Config> config);

		if(!isGetConfigSuccess)
		{
			LogManager.Info($"ConfigManager: Config \"{name}\" is not found. ...");
			LogManager.Info($"ConfigManager: Searching for default config to activate it...");

			try
			{
				LogManager.Info($"ConfigManager: {Utils.Stringify(Configs.Count())}");
			}
			catch(Exception exception)
			{
				LogManager.Error(exception);
			}

			bool isGetDefaultConfigSuccess = Configs.TryGetValue(Constants.DEFAULT_CONFIG, out JsonDatabase<Config> defaultConfig);

			if(!isGetDefaultConfigSuccess)
			{
				LogManager.Info($"ConfigManager: Default config is not found. Creating it...");

				defaultConfig = new(Constants.CONFIGS_PATH, Constants.DEFAULT_CONFIG);
				defaultConfig.Data.Name = Constants.DEFAULT_CONFIG;
				defaultConfig.Save();
				Configs[Constants.DEFAULT_CONFIG] = defaultConfig;

				LogManager.Info($"ConfigManager: Default config is created!");

				ActivateConfig(defaultConfig);
				return;
			}

			LogManager.Info($"ConfigManager: Default config is found!");

			ActivateConfig(defaultConfig);
			return;
		}

		LogManager.Info($"ConfigManager: Config \"{name}\" is found!");

		ActivateConfig(config);
	}

	public JsonDatabase<Config> InitializeConfig(string name, Config configToClone = null)
	{
		LogManager.Info($"ConfigManager: Initializing config \"{name}\"...");

		JsonDatabase<Config> config = new(Constants.CONFIGS_PATH, name, configToClone);
		config.Data.Name = name;
		config.Save();

		config.Changed += OnConfigFileChanged;
		config.Created += OnConfigFileCreated;
		config.RenamedFrom += OnConfigFileRenamedFrom;
		config.RenamedTo += OnConfigFileRenamedTo;
		config.Deleted += OnConfigFileDeleted;
		config.Error += OnConfigFileError;

		Configs[name] = config;

		LogManager.Info($"ConfigManager: Config \"{name}\" is initialized!");

		return config;
	}

	public void DuplicateConfig(string newConfigName)
	{

	}

	public void Dispose()
	{
		LogManager.Info("ConfigManager: Disposing...");

		ConfigWatcherInstance.Dispose();
		CurrentConfigInstance.Dispose();

		foreach(var config in Configs)
		{
			config.Value.Dispose();
		}

		LogManager.Info("ConfigManager: Disposed!");
	}

	private void LoadCurrentConfig()
	{
		LogManager.Info("ConfigManager: Loading current config...");

		CurrentConfigInstance = new(Constants.PLUGIN_DATA_PATH, Constants.CURRENT_CONFIG);

		CurrentConfigInstance.Changed += OnCurrentConfigChanged;
		CurrentConfigInstance.Created += OnCurrentConfigCreated;
		CurrentConfigInstance.RenamedFrom += OnCurrentConfigRenamedFrom;
		CurrentConfigInstance.RenamedTo += OnCurrentConfigRenamedTo;
		CurrentConfigInstance.Deleted += OnCurrentConfigDeleted;
		CurrentConfigInstance.Error += OnCurrentConfigError;

		ActivateConfig(CurrentConfigInstance.Data.ConfigName);

		LogManager.Info("ConfigManager: Current config loaded!");
	}

	private void LoadAllConfigs()
	{
		try
		{
			LogManager.Info("ConfigManager: Loading all configs...");

			Directory.CreateDirectory(Path.GetDirectoryName(Constants.CONFIGS_PATH));

			string[] allConfigFilePathNames = Directory.GetFiles(Constants.CONFIGS_PATH);

			if(allConfigFilePathNames.Length == 0)
			{
				InitializeConfig(Constants.DEFAULT_CONFIG);
				return;
			}

			foreach(var configFilePathName in allConfigFilePathNames)
			{
				string name = Path.GetFileNameWithoutExtension(configFilePathName);
				InitializeConfig(name);
			}

			LogManager.Info("ConfigManager: Loading all configs is done!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private void OnCurrentConfigChanged(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file changed.");
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigCreated(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file created.");
		CurrentConfigInstance.Load();
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigRenamedFrom(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file renamed from.");
		CurrentConfigInstance.Save();
	}

	private void OnCurrentConfigRenamedTo(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file renamed to.");
		CurrentConfigInstance.Load();
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigDeleted(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file deleted.");
		CurrentConfigInstance.Save();
	}

	private void OnCurrentConfigError(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file throw an error.");
		CurrentConfigInstance.Save();
	}

	private void OnConfigFileChanged(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file changed.");
	}

	private void OnConfigFileCreated(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file created.");
	}

	private void OnConfigFileRenamedFrom(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file renamed from.");
	}

	private void OnConfigFileRenamedTo(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file renamed to.");
	}

	private void OnConfigFileDeleted(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file deleted.");
	}

	private void OnConfigFileError(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Config file throw an error.");
	}

	private void OnActiveConfigChanged()
	{
		Utils.EmitEvents(this, ActiveConfigChanged);
	}
}
