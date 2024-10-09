using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class ImGuiManager
{
	private static readonly Lazy<ImGuiManager> _lazy = new(() => new ImGuiManager());

	public static ImGuiManager Instance => _lazy.Value;

	public float ComboBoxWidth { get; set; } = 100f;

	public bool IsOpened { get => _isOpened; set => _isOpened = value; }
	private bool _isOpened = false;

	private bool IsForceModInfoOpen { get; set; } = true;

	private ImGuiManager()
	{
		LogManager.Info("ImGuiManager: Initializing...");

		LogManager.Info("ImGuiManager: Initialized!");
	}

	public void Draw()
	{
		try
		{
			if(!IsOpened) return;

			var activeLocalization = LocalizationManager.Instance.ActiveLocalization.Data;

			var changed = false;

			ImGui.SetNextWindowPos(Constants.DEFAULT_WINDOW_POSITION, ImGuiCond.FirstUseEver);
			ImGui.SetNextWindowSize(Constants.DEFAULT_WINDOW_SIZE, ImGuiCond.FirstUseEver);

			ImGui.Begin($"{Constants.MOD_NAME} v{Constants.VERSION}", ref _isOpened);

			ComboBoxWidth = Constants.COMBOBOX_WIDTH_MULTIPLIER * ImGui.GetWindowSize().X;

			if(IsForceModInfoOpen) ImGui.SetNextItemOpen(true);

			if(ImGui.TreeNode(activeLocalization.ImGui.ModInfo))
			{
				ImGui.Text(activeLocalization.ImGui.MadeBy);
				ImGui.SameLine();
				ImGui.TextColored(Constants.MOD_AUTHOR_COLOR, Constants.MOD_AUTHOR);

				if(ImGui.Button(activeLocalization.ImGui.NexusMods)) Utils.OpenLink(Constants.NEXUSMODS_LINK);
				ImGui.SameLine();
				if(ImGui.Button(activeLocalization.ImGui.GitHubRepo)) Utils.OpenLink(Constants.GITHUB_REPO_LINK);

				if(ImGui.Button(activeLocalization.ImGui.Twitch)) Utils.OpenLink(Constants.TWITCH_LINK);
				ImGui.SameLine();
				if(ImGui.Button(activeLocalization.ImGui.Twitter)) Utils.OpenLink(Constants.TWITTER_LINK);
				ImGui.SameLine();
				if(ImGui.Button(activeLocalization.ImGui.ArtStation)) Utils.OpenLink(Constants.ARTSTATION_LINK);

				ImGui.Text(activeLocalization.ImGui.DonationMessage1);
				ImGui.Text(activeLocalization.ImGui.DonationMessage2);

				if(ImGui.Button(activeLocalization.ImGui.Donate)) Utils.OpenLink(Constants.STREAMELEMENTS_TIP_LINK);
				ImGui.SameLine();
				if(ImGui.Button(activeLocalization.ImGui.PayPal)) Utils.OpenLink(Constants.PAYPAL_LINK);
				ImGui.SameLine();
				if(ImGui.Button(activeLocalization.ImGui.BuyMeATea)) Utils.OpenLink(Constants.KOFI_LINK);

				ImGui.TreePop();
			}
			else
			{
				IsForceModInfoOpen = false;
			}

			ImGui.Separator();
			ImGui.NewLine();
			ImGui.Separator();
		}
		catch(Exception e)
		{
			LogManager.Error(e);
		}
	}
}
