using EnoPM.BetterVanilla.Buttons;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core.Data;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class LocalSettingsManager
{
    public readonly CustomSettingCategory Category;

    public readonly BoolSetting DisplayRolesAndTasksAfterDeath;
    public readonly BoolSetting AutoRejoinPreviousLobby;
    public readonly EnumSetting<SettingTeamPreferences> TeamPreference;
    public readonly EnumSetting<CustomButtonsManager.ButtonPositions> ModMenuButtonPosition;
    public readonly FloatSetting ZoomValueOnDeath;

    public readonly BoolSetting AllowModdedCosmetics;
    public readonly BoolSetting AutoFinishMyTasks;
    public readonly BoolSetting DisableInfosChecks;
    public readonly BoolSetting DisableEndGameChecks;
    public readonly BoolSetting DisableStartGamePlayerRequirement;
    public readonly EnumSetting<SettingTeamPreferences> ForcedTeamAssignment;
    
    public LocalSettingsManager()
    {
        Category = new CustomSettingCategory("LocalSettings");

        DisplayRolesAndTasksAfterDeath = Category.Bool("DisplayRolesAndTasksAfterDeath", "Display roles and tasks after death", true);
        
        AutoRejoinPreviousLobby = Category.Bool("AutoRejoinPreviousLobbyAfterGame", "Auto Play Again");
        
        TeamPreference = Category.Enum("TeamAssignmentPreference", "Team Assignment Preference", SettingTeamPreferences.Both, isEditableFunc: IsTeamPreferenceSelectionAllowed);
        TeamPreference.ValueChanged += OnTeamPreferenceChanged;

        ModMenuButtonPosition = Category.Enum("ModMenuButtonPosition", "Mod Menu Button Position", CustomButtonsManager.ButtonPositions.BottomLeft);
        ModMenuButtonPosition.ValueChanged += OnModMenuButtonPositionValueChanged;

        ZoomValueOnDeath = Category.Float("ZoomValueOnDeath", "Dezoom value on death", new NumberRange(3f, 12f), suffix: "x", isEditableFunc: IsZoomAllowed);
        ZoomValueOnDeath.ValueChanged += OnZoomValueChanged;

        AllowModdedCosmetics = Category.Bool("AllowModdedCosmetics", "Enable Modded Cosmetics");
        AllowModdedCosmetics.LockedByHash("BBB7513CC9488F1FFBCF53A294B3D15FACF4457BC6019B8200A48952232BCC59");

        AutoFinishMyTasks = Category.Bool("AutoFinishMyTasks", "Auto Finish My Tasks After Death");
        AutoFinishMyTasks.LockedByHash("1C27A7F613EA4EFB37B3A18F8B2DC40E6167EFFFD34D1FA139199199C427860F");
        
        DisableInfosChecks = Category.Bool("DisableInfosChecks", "Disable Infos Checks");
        DisableInfosChecks.LockedByHash("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        
        DisableEndGameChecks = Category.Bool("DisableEndGameChecks", "Disable End Game Checks");
        DisableEndGameChecks.LockedByHash("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        
        DisableStartGamePlayerRequirement = Category.Bool("DisableStartGamePlayerRequirement", "Disable Start Game Player Requirement");
        DisableStartGamePlayerRequirement.LockedByHash("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        
        ForcedTeamAssignment = Category.Enum("ForcedTeamAssignment", "Forced Team Assignment", SettingTeamPreferences.Both);
        ForcedTeamAssignment.LockedByHash("6C86869BCDDD51E006515334CCBD1CD2F374ADC7369D28F69C29E24D4D0B9EE0");
        ForcedTeamAssignment.ValueChanged += OnForcedTeamAssignmentChanged;
    }

    private static void OnZoomValueChanged(float value)
    {
        
    }

    private static void OnModMenuButtonPositionValueChanged(CustomButtonsManager.ButtonPositions buttonPosition)
    {
        if (!CustomButtonsManager.Instance || !ModMenuHudButton.Instance)
        {
            return;
        }
        if (CustomButtonsManager.Instance.Positions.TryGetValue(buttonPosition, out var container))
        {
            Plugin.Logger.LogMessage($"Trying to set position to {buttonPosition.ToString()} {container.transform.name}");
            ModMenuHudButton.Instance.UpdateParent(container.transform);
        }
    }

    private static void OnTeamPreferenceChanged(SettingTeamPreferences value)
    {
        if (!AmongUsClient.Instance || !PlayerControl.LocalPlayer) return;
        PlayerControl.LocalPlayer.RpcSetTeamAssignmentPreference(value);
    }
    
    private void OnForcedTeamAssignmentChanged(SettingTeamPreferences value)
    {
        if (!AmongUsClient.Instance || !PlayerControl.LocalPlayer || ForcedTeamAssignment.IsLocked()) return;
        PlayerControl.LocalPlayer.RpcSetForcedTeamAssignment(value);
    }

    private static bool IsZoomAllowed()
    {
        return Utils.AmDead;
    }

    private static bool IsTeamPreferenceSelectionAllowed()
    {
        return ModSettings.Host.TeamPreferencesAllowed;
    }
}