using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Configuration;
using SharpPluginLoader.Core.Entities;
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
			LogManager.Error(exception);
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
			LogManager.Error(exception);
		}
	}

	public void OnImGuiFreeRender()
	{
		try
		{
			if(!IsInitialized) return;
			if(Renderer.MenuShown) ImGuiManager.Instance.Draw();

			//LogManager.Info("   ");
			//foreach (var largeMonster in LargeMonsterManager.Instance.LargeMonsters)
			//{
			//	LogManager.Info($"Large Monster: {largeMonster.Key.Name}");
			//}
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public void OnMonsterInitialized(Monster monsterRef)
	{
		try
		{
			Task.Run(() => MonsterManager.Instance.OnMonsterInitialized(monsterRef));
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public void OnMonsterDeath(Monster monsterRef)
	{
		try
		{
			Task.Run(() => MonsterManager.Instance.OnMonsterDeath(monsterRef));
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	public void OnMonsterDestroy(Monster monsterRef)
	{
		try
		{
			Task.Run(() => MonsterManager.Instance.OnMonsterDestroy(monsterRef));
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}


	private void Init() {
		try
		{
			LogManager.Info("Managers: Initializing...");

			ConfigManager configManager = ConfigManager.Instance;
			LocalizationManager localizationManager = LocalizationManager.Instance;
			ImGuiManager imGuiManager = ImGuiManager.Instance;

			LargeMonsterManager largeMonsterManager = LargeMonsterManager.Instance;
			MonsterManager monsterManager = MonsterManager.Instance;


			configManager.Initialize();
			localizationManager.Initialize();
			imGuiManager.Initialize();

			largeMonsterManager.Initialize();
			monsterManager.Initialize();


			IsInitialized = true;

			LogManager.Info("Managers: Initialized!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}
}
