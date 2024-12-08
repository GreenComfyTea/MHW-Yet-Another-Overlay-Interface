using ImGuiNET;
using SharpPluginLoader.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class ConfigCustomization
{
	private int _activeConfigIndex = 0;
	private string _configNameInput = string.Empty;

	private string[] ConfigNames { get; set; } = [];


	public ConfigCustomization()
	{

		ConfigManager configManager = ConfigManager.Instance;
		configManager.ActiveConfigChanged += OnAnyConfigChanged;

		OnAnyConfigChanged(configManager, EventArgs.Empty);
	}

	public bool RenderImGui()
	{
		ConfigManager configManager = ConfigManager.Instance;
		ImGuiLocalization localization = LocalizationManager.Instance.ActiveLocalization.Data.ImGui;

		bool isChanged = false;

		if(ImGui.TreeNode(localization.Config))
		{
			bool isActiveConfigChanged = ImGui.Combo(localization.ActiveConfig, ref _activeConfigIndex, ConfigNames, ConfigNames.Length);
			if(isActiveConfigChanged)
			{
				isChanged |= isActiveConfigChanged;

				configManager.ActivateConfig(ConfigNames[_activeConfigIndex]);
			}
			
			ImGui.InputText(localization.NewConfigName, ref _configNameInput, Constants.MAX_CONFIG_NAME_LENGTH);

			if(ImGui.Button(localization.New))
			{
				if(_configNameInput != string.Empty && !ConfigNames.Contains(_configNameInput))
				{
					isChanged |= isActiveConfigChanged;

					configManager.ConfigWatcherInstance.Disable();
					var newConfig = configManager.InitializeConfig(_configNameInput);
					configManager.ConfigWatcherInstance.DelayedEnable();

					configManager.ActivateConfig(newConfig);
				}
			}

			ImGui.SameLine();

			if(ImGui.Button(localization.Duplicate))
			{
				if(_configNameInput != string.Empty && !ConfigNames.Contains(_configNameInput))
				{
					isChanged |= isActiveConfigChanged;

					configManager.ConfigWatcherInstance.Disable();
					JsonDatabase<Config> newConfig = configManager.InitializeConfig(_configNameInput, configManager.ActiveConfig.Data);
					configManager.ConfigWatcherInstance.DelayedEnable();

					configManager.ActivateConfig(newConfig);
				}
			}

			ImGui.SameLine();

			if(ImGui.Button(localization.Rename))
			{
				if(_configNameInput != string.Empty && !ConfigNames.Contains(_configNameInput))
				{
					isChanged |= isActiveConfigChanged;

					configManager.ConfigWatcherInstance.Disable();
					JsonDatabase<Config> newConfig = configManager.InitializeConfig(_configNameInput, configManager.ActiveConfig.Data);
					configManager.ConfigWatcherInstance.DelayedEnable();

					configManager.ActivateConfig(newConfig);
				}

				isChanged |= isActiveConfigChanged;
			}

			ImGui.SameLine();

			if(ImGui.Button(localization.Reset))
			{
				isChanged |= isActiveConfigChanged;
			}

			ImGui.TreePop();
		}

		return isChanged;
	}

	private void OnAnyConfigChanged(object sender, EventArgs eventArgs)
	{
		ConfigManager configManager = ConfigManager.Instance;

		LogManager.Info($"ConfigCustomization: Config changed.{Utils.Stringify(configManager.Configs.Values.Select(config => config.Data.Name).ToArray())}");

		ConfigNames = configManager.Configs.Values.Select(config => config.Data.Name).ToArray();
		_activeConfigIndex = Array.IndexOf(ConfigNames, configManager.ActiveConfig.Data.Name);
	}
}
