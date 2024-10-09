using SharpPluginLoader.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Contacts;

namespace YetAnotherOverlayInterface;

internal sealed class LocalizationManager : IDisposable
{
	private static readonly Lazy<LocalizationManager> _lazy = new(() => new LocalizationManager());

	public static LocalizationManager Instance => _lazy.Value;

	public JsonDatabase<Localization> ActiveLocalization { get; set; }
	public JsonDatabase<Localization> DefaultLocalization { get; set; }
	public Dictionary<string, JsonDatabase<Localization>> Localizations { get; set; } = new();

	private LocalizationWatcher LocalizationWatcherInstance { get; }

	private LocalizationManager()
	{
		LogManager.Info("LocalizationManager: Initializing...");

		LoadAllLocalizations();
		ActivateLocalization(ConfigManager.Instance.ActiveConfig.Data.Localization);

		LocalizationWatcherInstance = new();

		LogManager.Info("LocalizationManager: Initialized!");
	}

	public void ActivateLocalization(JsonDatabase<Localization> localization)
	{
		LogManager.Info($"LocalizationManager: Activating localization \"{localization.Name}\"...");

		ActiveLocalization = localization;

		LogManager.Info($"LocalizationManager: Localization \"{localization.Name}\" is activated!");
	}

	public void ActivateLocalization(string name)
	{
		LogManager.Info($"LocalizationManager: Searching for localization \"{name}\" to activate it...");

		bool isGetConfigSuccess = Localizations.TryGetValue(name, out JsonDatabase<Localization> localization);

		if(!isGetConfigSuccess)
		{
			LogManager.Info($"LocalizationManager: Config \"{name}\" is not found. ...");
			LogManager.Info($"LocalizationManager: Activating default localization...");

			ActivateLocalization(DefaultLocalization);
			return;
		}

		LogManager.Info($"LocalizationManager: Localization \"{name}\" is found!");

		ActivateLocalization(localization);
	}

	public void LoadLocalization(string name)
	{
		LogManager.Info($"LocalizationManager: Loading localization \"{name}\"...");

		JsonDatabase<Localization> newLocalization = new(Constants.LOCALIZATIONS_PATH, name);
		newLocalization.Data.IsoCode = name;
		newLocalization.Save();
		Localizations[name] = newLocalization;

		LogManager.Info($"LocalizationManager: Localization \"{name}\" is loaded!");
	}

	public void Dispose()
	{
		LogManager.Info("LocalizationManager: Disposing...");

		LocalizationWatcherInstance.Dispose();

		foreach(var localization in Localizations)
		{
			localization.Value.Dispose();
		}

		LogManager.Info("LocalizationManager: Disposed!");
	}

	private void InitializeDefaultLocalization()
	{
		LogManager.Info($"LocalizationManager: Initializing default localization...");

		JsonDatabase<Localization> defaultLocalization = new(Constants.LOCALIZATIONS_PATH, Constants.DEFAULT_LOCALIZATION);
		defaultLocalization.Data = new Localization();
		defaultLocalization.Save();
		Localizations[Constants.DEFAULT_CONFIG] = defaultLocalization;
		DefaultLocalization = defaultLocalization;

		LogManager.Info($"LocalizationManager: Default localization is initialized!");
	}

	private void LoadAllLocalizations()
	{
		try
		{
			LogManager.Info("LocalizationManager: Loading all localizations...");

			Directory.CreateDirectory(Path.GetDirectoryName(Constants.LOCALIZATIONS_PATH));

			string[] allConfigFilePathNames = Directory.GetFiles(Constants.LOCALIZATIONS_PATH);

			foreach(var configFilePathName in allConfigFilePathNames)
			{

				string name = Path.GetFileNameWithoutExtension(configFilePathName);

				if(name == Constants.DEFAULT_LOCALIZATION) continue;

				LoadLocalization(name);
			}

			InitializeDefaultLocalization();

			LogManager.Info("LocalizationManager: Loading all localizations is done!");
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
		}
	}
}
