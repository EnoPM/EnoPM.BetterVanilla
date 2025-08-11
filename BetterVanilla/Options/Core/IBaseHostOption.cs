namespace BetterVanilla.Options.Core;

public interface IBaseHostOption
{
    public ViewSettingsInfoPanel? ViewSettingBehaviour { get; }
    public BaseGameSetting GetGameSetting();
    public OptionBehaviour? GetOptionBehaviour();
    public void OnViewBehaviourCreated(ViewSettingsInfoPanel viewBehaviour);
    public void UpdateValueFromSettingBehaviour();
    public void UpdateBehaviours();
    public void SetViewSettingBehaviourInfo(ViewSettingsInfoPanel viewBehaviour, int maskLayer);
}