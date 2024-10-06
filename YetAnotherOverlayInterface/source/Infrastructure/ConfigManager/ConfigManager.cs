using SharpPluginLoader.Core.Configuration;
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
	private CurrentConfig CurrentConfigIntance { get; set; }
	private CurrentConfigWatcher CurrentConfigWatcherInstance { get; set; }

	public Dictionary<string, Config> Configs { get; set; } = [];
	public Config ActiveConfig { get; set; }
	public ConfigWatcher ConfigWatcherInstance { get; set; }

	private ConfigManager()
	{
		LogManager.Instance.Info("ConfigManager: Initializing...");

		// Create folder hierarchy if it doesn't exist
		Directory.CreateDirectory(Constants.CONFIGS_PATH);


		// Default initialization
		CurrentConfigWatcherInstance = new();
		ConfigWatcherInstance = new();

		CurrentConfigWatcherInstance.Disable();
		ConfigWatcherInstance.Disable();

		CurrentConfigIntance = new();
		Configs = [];
		Config defaultConfig = new();
		Configs.Add(Constants.DEFAULT_CONFIG, defaultConfig);

		LoadAllConfigs();

		// If current config file doesn't exist - create it and use default config
		if(!File.Exists(Constants.CURRENT_CONFIG_FILE_PATH_NAME))
		{
			LogManager.Instance.Info($"ConfigManager: Current Config Doesn't Exist. Using Default Config.");

			SetActiveConfig(defaultConfig);

			LogManager.Instance.Info($"ConfigManager: Initialized!");
			return;
		}

		LoadCurrentConfig();

		CurrentConfigWatcherInstance.Enable();
		ConfigWatcherInstance.Enable();

		LogManager.Instance.Info("ConfigManager: Initialized!");

	}

	public void SetActiveConfig(Config config)
	{
		LogManager.Instance.Info($"ConfigManager: Setting Active Config to \"{config.Name}\"...");

		ActiveConfig = config;

		ConfigWatcherInstance.Disable();
		ActiveConfig.Save();
		ConfigWatcherInstance.Enable();

		CurrentConfigIntance.currentConfig = config.Name;
		CurrentConfigIntance.Save();

		LogManager.Instance.Info($"ConfigManager: Active Config Is Set to \"{config.Name}\"!");
	}

	public void SetActiveConfig(string name)
	{
		LogManager.Instance.Info($"ConfigManager: Searching for Config \"${name}\"...");

		Config config;
		bool configExists = Configs.TryGetValue(name, out config);

		if(!configExists)
		{
			LogManager.Instance.Info($"ConfigManager: Config \"${name}\" Not Found!");
			return;
		}

		LogManager.Instance.Info($"ConfigManager: Config \"${name}\" Found!");
		SetActiveConfig(config);
	}

	public void SaveCurrentConfig()
	{
		LogManager.Instance.Info("ConfigManager: Saving Current Config...");

		CurrentConfigIntance.Save();

		LogManager.Instance.Info("ConfigManager: Current Config Saved!");
	}

	public void LoadCurrentConfig()
	{
		LogManager.Instance.Info("ConfigManager: Loading Current Config...");

		CurrentConfig newCurrentConfig = CurrentConfig.Load();

		if(newCurrentConfig == null)
		{
			LogManager.Instance.Info("ConfigManager: Loading Current Config Failed!");
			CurrentConfigIntance.Save();
			return;
		}

		Config config;
		bool configExists = Configs.TryGetValue(newCurrentConfig.currentConfig, out config);

		if(!configExists)
		{
			LogManager.Instance.Info("ConfigManager: Loading Current Config Failed!");
			CurrentConfigIntance.Save();
			return;
		}

		SetActiveConfig(config);

		LogManager.Instance.Info("ConfigManager: Current Config Loaded!");
	}

	public void LoadAllConfigs()
	{
		try
		{
			LogManager.Instance.Info("ConfigManager: Loading All Configs...");

			string[] allConfigFileNamePaths = Directory.GetFiles(Constants.CONFIGS_PATH);

			foreach(var configFileNamePath in allConfigFileNamePaths)
			{
				string configFileName = Path.GetFileNameWithoutExtension(configFileNamePath);

				Config newConfig = Config.Load(Path.GetFileName(configFileName));

				if(newConfig == null)
				{
					LogManager.Instance.Info($"ConfigManager: Loading Config {configFileName} Failed!");
					continue;
				}

				Configs[newConfig.Name] = newConfig;
			}

			LogManager.Instance.Info("ConfigManager: All Configs Loaded!");
		}
		catch(Exception e)
		{
			LogManager.Instance.Info($"ConfigManager: Loading All Configs Failed! Error: {e.Message}");
			return;
		}
	}

	public void LoadConfig(string name)
	{
		LogManager.Instance.Info($"ConfigManager: Loading Config \"{name}\"...");

		Config newConfig = Config.Load(name);

		if(newConfig == null)
		{
			LogManager.Instance.Info($"ConfigManager: Loading Config \"{name}\" Failed!");

			Config oldConfig;
			bool oldConfigExists = Configs.TryGetValue(name, out oldConfig);

			if(oldConfigExists) oldConfig.Save();
			return;
		}

		Configs[name] = newConfig;
		if(ActiveConfig.Name == name) SetActiveConfig(newConfig);

		LogManager.Instance.Info($"ConfigManager: Config \"{name}\" Loaded!");
	}

	public void SaveConfig(Config config)
	{ 
		LogManager.Instance.Info($"ConfigManager: Saving Config \"{config.Name}\"...");

		ConfigWatcherInstance.Disable();
		config.Save();
		ConfigWatcherInstance.Enable();

		LogManager.Instance.Info($"ConfigManager: Config \"{config.Name}\" Saved!");
	
	}

	public void SaveConfig(string name)
	{
		LogManager.Instance.Info($"ConfigManager: Searching for Config \"{name}\"...");

		Config config;
		bool configExists = Configs.TryGetValue(name, out config);

		if(!configExists)
		{
			LogManager.Instance.Info($"ConfigManager: Config \"{name}\" Not Found!");
			return;
		}

		LogManager.Instance.Info($"ConfigManager: Config \"{name}\" Foind!");

		SaveConfig(config);
	}

	public void RemoveConfig(Config config)
	{
		LogManager.Instance.Info($"ConfigManager: Removing Config \"{config.Name}\"...");

		try
		{
			Configs.Remove(config.Name);
			string configFileNamePath = $"{Constants.CONFIGS_PATH}\\{config.Name}.json";
			if(File.Exists(configFileNamePath))
			{
				ConfigWatcherInstance.Disable();
				File.Delete(configFileNamePath);
				ConfigWatcherInstance.Enable();
			}

			LogManager.Instance.Info($"ConfigManager: \"{config.Name}\" Removed!");

			if(ActiveConfig.Name != config.Name) return;

			if(Configs.Count == 0)
			{
				Config defaultConfig = new();
				Configs.Add(Constants.DEFAULT_CONFIG, defaultConfig);
				SetActiveConfig(defaultConfig);

				return;
			}

			Config newActiveConfig = Configs.First().Value;
			SetActiveConfig(newActiveConfig);
		}
		catch(Exception e)
		{
			LogManager.Instance.Info($"ConfigManager: Removing Config \"{config.Name}\" Failed! Error: {e.Message}");
			return;
		}
		
	}

	public void RemoveConfig(string name)
	{
		LogManager.Instance.Info($"ConfigManager: Searching for Config \"{name}\"...");

		Config config;
		bool configExists = Configs.TryGetValue(name, out config);

		if(!configExists)
		{
			LogManager.Instance.Info($"ConfigManager: Config \"{name}\" Not Found!");
			return;
		}

		LogManager.Instance.Info($"ConfigManager: Config \"{name}\" Foind!");
		RemoveConfig(config);
	}
}
