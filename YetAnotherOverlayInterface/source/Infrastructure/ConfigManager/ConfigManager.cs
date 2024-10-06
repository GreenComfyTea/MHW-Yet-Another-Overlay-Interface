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
	public Config DefaultConfig { get; set; } = new();
	public Config ActiveConfig { get; set; }
	public string ActiveConfigName { get; set; } = Constants.DEFAULT_CONFIG;

	private ConfigManager()
	{
		LogManager.Instance.Info("ConfigManager: Initializing...");

		// Create folder hierarchy if it doesn't exist
		Directory.CreateDirectory(Constants.CONFIGS_PATH);


		// Default initialization
		CurrentConfigWatcherInstance = new();
		CurrentConfigIntance = new();
		Configs = [];
		DefaultConfig = new();
		Configs.Add(Constants.DEFAULT_CONFIG, DefaultConfig);

		LoadAllConfigs();

		// If current config file doesn't exist - create it and use default config
		if(!File.Exists(Constants.CURRENT_CONFIG_FILE_PATH_NAME))
		{
			LogManager.Instance.Info($"ConfigManager: Current Config Doesn't Exist.");

			SetActiveConfig(DefaultConfig);

			LogManager.Instance.Info($"ConfigManager: Initialization Done!");
			return;
		}

		LoadCurrentConfig();

		LogManager.Instance.Info("ConfigManager: Initialization Done!");

	}

	public void SetActiveConfig(Config config)
	{
		LogManager.Instance.Info($"ConfigManager: Setting Active Config to {config.Name}...");

		ActiveConfig = config;
		ActiveConfigName = config.Name;

		CurrentConfigIntance.currentConfig = config.Name;
		CurrentConfigIntance.Save();

		LogManager.Instance.Info($"ConfigManager: Setting Active Config to {config.Name} Done!");
	}

	public void SetActiveConfig(string name)
	{
		LogManager.Instance.Info($"ConfigManager: Searching for Config ${name}...");

		Config config;
		bool configExists = Configs.TryGetValue(name, out config);

		if(!configExists)
		{
			LogManager.Instance.Info($"ConfigManager: Config ${name} not found!");
			return;
		}

		SetActiveConfig(config);
	}

	public void SaveCurrentConfig()
	{
		LogManager.Instance.Info("ConfigManager: Saving Current Config...");

		CurrentConfigIntance.Save();

		LogManager.Instance.Info("ConfigManager: Saving Current Config Done!");
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

		LogManager.Instance.Info("ConfigManager: Loading Current Config Done!");
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

				Configs.Add(newConfig.Name, newConfig);
			}

			LogManager.Instance.Info("ConfigManager: Loading All Configs Done!");
		}
		catch(Exception e)
		{
			LogManager.Instance.Info("ConfigManager: Loading All Configs Failed!");
			return;
		}
	}

	public void LoadConfig(string name)
	{

	}
}
