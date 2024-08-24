using BetterVanilla.Core.Options;

namespace BetterVanilla.Core.Extensions;

public static class ViewSettingsInfoPanelExtensions
{
    public static void CustomSetInfoCheckbox(this ViewSettingsInfoPanel panel, int maskLayer, BaseHostOption option)
    {
        panel.titleText.SetText(option.Title);
        panel.settingText.SetText(string.Empty);
        panel.checkMarkOff.gameObject.SetActive(!option.GetBool());
        panel.background.gameObject.SetActive(true);
        panel.checkMark.gameObject.SetActive(option.GetBool());
        panel.SetMaskLayer(maskLayer);
    }
    
    public static void CustomSetInfo(this ViewSettingsInfoPanel panel, int maskLayer, BaseHostOption option)
    {
        panel.titleText.SetText(option.Title);
        panel.settingText.SetText(option.GetValueString());
        panel.disabledBackground.gameObject.SetActive(false);
        panel.background.gameObject.SetActive(true);
        panel.SetMaskLayer(maskLayer);
    }
}