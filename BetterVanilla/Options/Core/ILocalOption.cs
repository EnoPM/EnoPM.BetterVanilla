using BetterVanilla.Options.Components;

namespace BetterVanilla.Options.Core;

public interface ILocalOption<TOptionUi> where TOptionUi : BaseOptionUi
{
    public TOptionUi? UiOption { get; }
    
    public void SetUiOption(TOptionUi option);
    public void RefreshUiOption();
    public void RefreshUiLock();
    public void RefreshUiVisibility();
}