using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class CurrentConfig
{
	public string currentConfig { get; set; } = Constants.DEFAULT_CONFIG;

	public CurrentConfig()
	{
	}

	public CurrentConfig Duplicate()
	{
		CurrentConfig duplicateCurrentConfig = new();
		duplicateCurrentConfig.currentConfig = currentConfig;

		return duplicateCurrentConfig;
	}

	public void Reset()
	{
		currentConfig = Constants.DEFAULT_CONFIG;
		Save();
	}

	public void Save()
	{
		LogManager.Instance.Info("CurrentConfig: Saving...");

		JsonManager.SearializeToFile(Constants.CURRENT_CONFIG_FILE_PATH_NAME, this);

		LogManager.Instance.Info("CurrentConfig: Saving Done!");
	}

	public static CurrentConfig Load()
	{
		LogManager.Instance.Info("CurrentConfig: Loading...");

		CurrentConfig currentConfig = JsonManager.ReadFromFile<CurrentConfig>(Constants.CURRENT_CONFIG_FILE_PATH_NAME);

		if(currentConfig == null)
		{
			LogManager.Instance.Info("CurrentConfig: Loading Failed!");
			return null;
		}

		return currentConfig;
	}
}
