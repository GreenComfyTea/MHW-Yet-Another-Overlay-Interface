using JsonFlatFileDataStore;
using SharpPluginLoader.Core;

namespace YetAnotherOverlayInterface;

public class Employee
{
	public int Id { get; set; } = 0;
	public string Name { get; set; } = "MDSJHSDJK";
	public int Age { get; set; } = 18;
}

public class YetAnotherOverlayInterfacePlugin: IPlugin
{
	public string Name => $"{Constants.MOD_NAME} v{Constants.VERSION}";
	public string Author => Constants.MOD_AUTHOR;

	public PluginData Initialize()
	{
		return new PluginData();
	}

	public void OnLoad()
	{
		try
		{
			ConfigManager configManager = ConfigManager.Instance;
		}
		catch(Exception e)
		{
			LogManager.Info(e.Message);
		}
	}
}
