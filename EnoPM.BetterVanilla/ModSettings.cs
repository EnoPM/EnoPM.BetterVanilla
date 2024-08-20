using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core.Settings;

namespace EnoPM.BetterVanilla;

public static class ModSettings
{
    public static readonly HostSettingsManager Host;
    public static readonly LocalSettingsManager Local;

    static ModSettings()
    {
        Host = new HostSettingsManager();
        Local = new LocalSettingsManager();
    }

    public static void OnSettingsTabControllerReady(SettingsTabController controller)
    {
        var category = CustomSettingCategory.GetCategory(controller.categoryId);
        if (category == null) return;
        foreach (var setting in category.Settings)
        {
            setting.CreateSettingUi(controller);
        }
    }
}