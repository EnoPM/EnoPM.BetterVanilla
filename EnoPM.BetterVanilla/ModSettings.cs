using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Data;

namespace EnoPM.BetterVanilla;

public static class ModSettings
{
    public static readonly CustomSettingCategory LocalCategory;
    public static readonly CustomSettingCategory HostCategory;
    public static readonly CustomSettingCategory RolesCategory;
    
    public static readonly BoolSetting ShowTasksAndRoleAfterDeath;
    public static readonly EnumSetting<SettingTeamPreferences> TeamPreference;
    public static readonly FloatSetting Cooldown;

    public static readonly VanillaSettingsManager VanillaSettings;

    static ModSettings()
    {
        LocalCategory = new CustomSettingCategory("LocalSettings");
        HostCategory = new CustomSettingCategory("HostSettings", IsHostSettingsEditable);
        RolesCategory = new CustomSettingCategory("RolesSettings");
        
        ShowTasksAndRoleAfterDeath = LocalCategory.Bool("ShowTasksAndRoleAfterDeath", "Display tasks and role");
        TeamPreference = LocalCategory.Enum("TeamAssignmentPreference", "Team assignment preference", SettingTeamPreferences.Both);
        Cooldown = LocalCategory.Float("Cooldown", "My Float Setting", new NumberRange(10f, 120f), 2.5f, suffix: "s");

        VanillaSettings = new VanillaSettingsManager(HostCategory);
    }

    public static void InitUi(SettingsTabController controller)
    {
        var category = CustomSettingCategory.GetCategory(controller.categoryId);
        if (category == null) return;
        foreach (var setting in category.Settings)
        {
            setting.CreateSettingUi(controller);
        }
    }

    private static bool IsHostSettingsEditable() => AmongUsClient.Instance && AmongUsClient.Instance.AmHost;
    
    
}