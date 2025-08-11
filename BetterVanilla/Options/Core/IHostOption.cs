namespace BetterVanilla.Options.Core;

public interface IHostOption<out TGameSetting, TOptionBehaviour> : IBaseHostOption
where TGameSetting : BaseGameSetting
where TOptionBehaviour : OptionBehaviour
{
    public TGameSetting GameSetting { get; }
    public TOptionBehaviour? SettingBehaviour { get; }
    public void OnBehaviourCreated(TOptionBehaviour behaviour);
    
}