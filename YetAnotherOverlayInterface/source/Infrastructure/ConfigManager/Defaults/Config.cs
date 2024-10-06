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
		LogManager.Instance.Info($"{Name} Config: Saving...");

		JsonManager.SearializeToFile($"{Constants.CONFIGS_PATH}\\${Name}.json", this);

		LogManager.Instance.Info($"{Name} Config: Saving Done!");
	}

	public static Config Load(string name)
	{
		LogManager.Instance.Info($"{name} Config: Loading...");

		Config config = JsonManager.ReadFromFile<Config>($"{Constants.CONFIGS_PATH}\\${name}.json");

		if(config == null)
		{
			LogManager.Instance.Info($"{name} Config: Loading Failed!");
			return null;
		}

		config.Name = name;
		config.Save();

		return config;
	}
}
