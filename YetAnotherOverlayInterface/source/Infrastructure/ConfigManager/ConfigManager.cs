using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YetAnotherOverlayInterface;

internal class ConfigManager
{
	private static readonly Lazy<ConfigManager> _lazy = new(() => new ConfigManager());
	public static ConfigManager Instance => _lazy.Value;

	public JsonDatabase<CurrentConfig> CurrentConfigInstance { get; set; }

	public JsonDatabase<Config> ActiveConfig { get; set; }
	public Dictionary<string, JsonDatabase<Config>> Configs { get; set; } = [];


	private ConfigManager()
	{
		LogManager.Info("ConfigManager: Initializing...");

		LoadAllConfigs();
		LoadCurrentConfig();
		
		//CurrentConfig.Created += OnCurrentConfigCreated;
		//CurrentConfig.RenamedTo += OnCurrentConfigRenamed;
		//CurrentConfig.Deleted += OnCurrentConfigDeleted;
		//CurrentConfig.Error += OnCurrentConfigError;

		LogManager.Info("ConfigManager: Initialized!");
	}

	public void ActivateConfig(JsonDatabase<Config> config)
	{
		LogManager.Info($"ConfigManager: Activating config \"{config.Name}\"...");
		
		ActiveConfig = config;
		CurrentConfigInstance.Data.ConfigName = config.Name;
		CurrentConfigInstance.Save();

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
				JsonDatabase<Config> defaultConfig = new(Constants.CONFIGS_PATH, Constants.DEFAULT_CONFIG);
				defaultConfig.Data.Name = Constants.DEFAULT_CONFIG;
				defaultConfig.Save();
				Configs[Constants.DEFAULT_CONFIG] = defaultConfig;

				return;
			}

			foreach(var configFilePathName in allConfigFilePathNames)
			{
				string name = Path.GetFileNameWithoutExtension(configFilePathName);

				LogManager.Info($"ConfigManager: Loading config \"{name}\"...");

				JsonDatabase<Config> newConfig = new(Constants.CONFIGS_PATH, name);
				newConfig.Data.Name = name;
				newConfig.Save();
				Configs[name] = newConfig;

				LogManager.Info($"ConfigManager: Config \"{name}\" is loaded!");
			}

			LogManager.Info("ConfigManager: Loading all configs is done!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	private void OnCurrentConfigChanged(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file changed...");
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigCreated(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file created...");
		CurrentConfigInstance.Load();
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigRenamedFrom(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file renamed from...");
		CurrentConfigInstance.Save();
	}

	private void OnCurrentConfigRenamedTo(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file renamed to...");
		CurrentConfigInstance.Load();
		ActivateConfig(CurrentConfigInstance.Data.ConfigName);
	}

	private void OnCurrentConfigDeleted(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file deleted...");
		CurrentConfigInstance.Save();
	}

	private void OnCurrentConfigError(object sender, EventArgs eventArgs)
	{
		LogManager.Info("ConfigManager: Current config file error...");
		CurrentConfigInstance.Save();
	}
}
