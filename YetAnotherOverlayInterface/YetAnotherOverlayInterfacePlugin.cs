using ImGuiNET;
using JsonFlatFileDataStore;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Configuration;
using SharpPluginLoader.Core.Rendering;

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

	private bool IsInitialized { get; set; } = false;

	public PluginData Initialize()
	{
		return new PluginData
		{
			ImGuiWrappedInTreeNode = false
		};
	}

	public void OnLoad()
	{
		try
		{
			Task.Run(Init);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	public void OnUnload()
	{
		LogManager.Info("Managers: Disposing...");

		ConfigManager.Instance.Dispose();
		LocalizationManager.Instance.Dispose();

		LogManager.Info("Managers: Disposed!");
	}

	public void OnImGuiRender()
	{
		try
		{
			if(!IsInitialized) return;
			if(ImGui.Button($"{Constants.MOD_NAME} v{Constants.VERSION}"))
			{
				ImGuiManager imGuiManager = ImGuiManager.Instance;
				imGuiManager.IsOpened = !imGuiManager.IsOpened;
			}
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	public void OnImGuiFreeRender()
	{
		try
		{
			if(!IsInitialized) return;
			if(!Renderer.MenuShown) return;

			ImGuiManager.Instance.Draw();
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}

	private void Init() {
		try
		{
			LogManager.Info("Managers: Initializing...");

			ConfigManager configManager = ConfigManager.Instance;
			LocalizationManager localizationManager = LocalizationManager.Instance;

			ImGuiManager imGuiManager = ImGuiManager.Instance;

			IsInitialized = true;

			LogManager.Info("Managers: Initialized!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}
}
