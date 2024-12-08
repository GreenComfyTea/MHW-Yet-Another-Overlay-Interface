using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class LocalizationInfo
{
	public string Name { get; set; } = "English";
	public string Translators { get; set; } = "GreenComfyTea";
}

internal class ImGuiLocalization
{
	// Mod Info
	public string ModInfo { get; set; } = "Mod Info";

	public string MadeBy { get; set; } = "Made by:";
	public string NexusMods { get; set; } = "Nexus Mods";
	public string GitHubRepo { get; set; } = "GitHub Repo";
	public string Twitch { get; set; } = "Twitch";
	public string Twitter { get; set; } = "Twitter";
	public string ArtStation { get; set; } = "ArtStation";
	public string DonationMessage1 { get; set; } = "If you like the mod, please consider making a small donation!";
	public string DonationMessage2 { get; set; } = "It would help me maintain existing mods and create new ones in the future!";
	public string Donate { get; set; } = "Donate";
	public string PayPal { get; set; } = "PayPal";
	public string BuyMeATea { get; set; } = "Buy Me a Tea";

	public string Config { get; set; } = "Config";

	public string ActiveConfig { get; set; } = "Active Config";
	public string NewConfigName { get; set; } = "New Config Name";
	public string New { get; set; } = "New";
	public string Duplicate { get; set; } = "Duplicate";
	public string Delete { get; set; } = "Delete";
	public string Reset { get; set; } = "Reset";
	public string Rename { get; set; } = "Rename";
}

internal class Localization
{
	[JsonIgnore]
	public string IsoCode { get; set; } = Constants.DEFAULT_LOCALIZATION;

	public LocalizationInfo LocalizationInfo { get; set; } = new();
	public ImGuiLocalization ImGui { get; set; } = new();



}
