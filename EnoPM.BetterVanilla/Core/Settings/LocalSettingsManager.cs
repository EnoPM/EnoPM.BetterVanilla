using EnoPM.BetterVanilla.Core.Data;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class LocalSettingsManager
{
    public readonly CustomSettingCategory Category;

    public readonly BoolSetting DisplayRolesAndTasksAfterDeath;
    public readonly BoolSetting AutoRejoinPreviousLobby;
    public readonly EnumSetting<SettingTeamPreferences> TeamPreference;
    
    public LocalSettingsManager()
    {
        Category = new CustomSettingCategory("LocalSettings");

        DisplayRolesAndTasksAfterDeath = Category.Bool("DisplayRolesAndTasksAfterDeath", "Display roles and tasks after death", true);
        AutoRejoinPreviousLobby = Category.Bool("AutoRejoinPreviousLobbyAfterGame", "Auto rejoin previous lobby");
        TeamPreference = Category.Enum("TeamAssignmentPreference", "Team assignment preference", SettingTeamPreferences.Both);
    }
}