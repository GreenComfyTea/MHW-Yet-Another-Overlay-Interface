using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class Config
{
	public string Name { get; set; }

	public Config(string name = Constants.DEFAULT_CONFIG)
	{
		Name = name;
	}

	public void Save()
	{
		LogManager.Instance.Info($"Config \"{Name}\": Saving...");

		JsonManager.SearializeToFile($"{Constants.CONFIGS_PATH}\\{Name}.json", this);

		LogManager.Instance.Info($"Config \"{Name}\": Saved!");
	}

	public static Config Load(string name)
	{
		LogManager.Instance.Info($"Config \"{name}\": Loading...");

		Config config = JsonManager.ReadFromFile<Config>($"{Constants.CONFIGS_PATH}\\{name}.json");

		if(config == null)
		{
			LogManager.Instance.Info($"Config \"{name}\": Loading Failed!");
			return null;
		}

		config.Name = name;

		LogManager.Instance.Info($"Config \"{name}\": Loaded!");

		config.Save();
		return config;
	}
}
