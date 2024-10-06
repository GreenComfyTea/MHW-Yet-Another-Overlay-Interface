using SharpPluginLoader.Core;

namespace YetAnotherOverlayInterface;

public class YetAnotherOverlayInterfacePlugin: IPlugin
{
	public string Name => $"{Constants.MOD_NAME} v{Constants.VERSION}";
	public string Author => $"{Constants.MOD_AUTHOR}";

	public PluginData Initialize()
	{
		return new PluginData();
	}

	public void OnLoad()
	{
		ConfigManager configManager = ConfigManager.Instance;
	}
}
