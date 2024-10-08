using System.Numerics;
using System.Text.Json;

namespace YetAnotherOverlayInterface;

internal static class Constants
{
	public const string MOD_AUTHOR = "GreenComfyTea";
	public const string MOD_NAME = "Yet Another Overlay Interface";
	public const string MOD_NAME_ABBREVIATION = "YAOI";
	public const string MOD_FOLDER_NAME = "YetAnotherOverlayInterface";

	public const string VERSION = "2.0.0";

	public const string PLUGIN_PATH = $@"nativePC\plugins\CSharp\{MOD_FOLDER_NAME}\";
	public const string PLUGIN_DATA_PATH = $@"{PLUGIN_PATH}data\";
	public const string PLUGIN_FONTS_PATH = $@"{PLUGIN_DATA_PATH}fonts\";
	public const string LOCALIZATIONS_PATH = $@"{PLUGIN_DATA_PATH}localizations\";

	public const string CURRENT_CONFIG = "current_config";
	public const string CURRENT_CONFIG_WITH_EXTENSION = $"{CURRENT_CONFIG}.json";
	public const string CURRENT_CONFIG_FILE_PATH_NAME = $"{PLUGIN_DATA_PATH}{CURRENT_CONFIG_WITH_EXTENSION}";

	public const string DEFAULT_CONFIG = "default";
	public const string DEFAULT_CONFIG_WITH_EXTENSION = $"{DEFAULT_CONFIG}.json";
	public const string DEFAULT_CONFIG_FILE_PATH_NAME = $"{PLUGIN_DATA_PATH}{DEFAULT_CONFIG_WITH_EXTENSION}";

	public const string CONFIGS_PATH = $@"{PLUGIN_DATA_PATH}configs\";

	public const string DEFAULT_LOCALIZATION = "en-US";

	public const float DRAG_FLOAT_SPEED = 0.1f;
	public const float DRAG_FLOAT_MAX = 15360f;
	public const float DRAG_FLOAT_MIN = -DRAG_FLOAT_MAX;
	public const string DRAG_FLOAT_FORMAT = "0.0";

	public const string NEXUSMODS_LINK = "https://nexusmods.com/monsterhunterworld/mods/...";
	public const string GITHUB_REPO_LINK = "https://github.com/GreenComfyTea/MHW-Yet-Another-Overlay-Interface";
	public const string NIGHTLY_LINK = "https://github.com/GreenComfyTea/MHW-Yet-Another-Overlay-Interface-Nightly/releases";
	public const string TWITCH_LINK = "https://twitch.tv/GreenComfyTea";
	public const string TWITTER_LINK = "https://twitter.com/GreenComfyTea";
	public const string ARTSTATION_LINK = "https://greencomfytea.artstation.com";
	public const string STREAMELEMENTS_TIP_LINK = "https://streamelements.com/greencomfytea/tip";
	public const string PAYPAL_LINK = "https://paypal.me/greencomfytea";
	public const string KOFI_LINK = "https://ko-fi.com/greencomfytea";

	public const string EMOJI_FONT = "NotoEmoji-Bold.ttf";

	public const int DUPLICATE_EVENT_TICK_THRESHOLD = 10000;

	public static readonly Vector4 MOD_AUTHOR_COLOR = new(0.702f, 0.851f, 0.424f, 1f);
	public static readonly Vector4 IMGUI_USERNAME_COLOR = new(0.5f, 0.710f, 1f, 1f);

	public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS_INSTANCE = new()
	{
		WriteIndented = true,
		AllowTrailingCommas = true,
		Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

}
