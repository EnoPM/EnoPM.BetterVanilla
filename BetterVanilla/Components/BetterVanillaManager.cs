using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Compiler;
using BetterVanilla.Components.Menu;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterVanillaManager : MonoBehaviour
{
    public static BetterVanillaManager Instance { get; private set; }

    private readonly Harmony _harmony = new (GeneratedProps.Guid);
    public readonly List<BetterPlayerControl> AllPlayers = [];
    public readonly Dictionary<int, TeamPreferences> AllTeamPreferences = [];
    public readonly Dictionary<int, TeamPreferences> AllForcedTeamAssignments = [];
    public DatabaseManager Database { get; private set; }
    public FeaturesManager Features { get; private set; }
    public ChatCommandsManager ChatCommands { get; private set; }
    public HostOptionsHolder HostOptions { get; private set; }
    public LocalOptionsHolder LocalOptions { get; private set; }
    public CheatersManager Cheaters { get; private set; }
    public XpManager Xp { get; private set; }
    public ModMenuButton MenuButton { get; private set; }
    private ModUpdater Updater { get; set; }
    public ModMenu Menu { get; private set; }
    public ZoomBehaviourManager ZoomBehaviour { get; set; }
    
    private void Awake()
    {
        if (Instance) throw new Exception($"{nameof(BetterVanillaManager)} must be a singleton");
        Instance = this;
        
        Database = AttachManager<DatabaseManager>();
        Features = AttachManager<FeaturesManager>();
        ChatCommands = AttachManager<ChatCommandsManager>();
        HostOptions = AttachManager<HostOptionsHolder>();
        LocalOptions = AttachManager<LocalOptionsHolder>();
        Cheaters = AttachManager<CheatersManager>();
        Xp = AttachManager<XpManager>();

        var uiBundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.ui");

        MenuButton = AttachComponent<ModMenuButton>(uiBundle, "Assets/Ui/Components/ModMenuButton.prefab");
        Updater = AttachComponent<ModUpdater>(uiBundle, "Assets/Ui/Windows/ModUpdaterUi.prefab");
        Menu = AttachComponent<ModMenu>(uiBundle, "Assets/Ui/Windows/ModMenuUi.prefab");
        
        uiBundle.Unload(false);

        GameEventManager.PlayerJoined += OnPlayerJoined;
        
        Ls.LogInfo($"Plugin {GeneratedProps.Name} v{GeneratedProps.Version} is loaded!");
    }
    
    private void Start()
    {
        _harmony.PatchAll();
        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        while (!ModManager.InstanceExists)
        {
            yield return new WaitForSeconds(1f);
        }
        
        ModManager.Instance.ShowModStamp();
        Updater.Initialize("EnoPM/EnoPM.BetterVanilla", "EnoPM.BetterVanilla.dll", "BetterVanilla.dll");
        
        Ls.LogInfo($"Plugin {GeneratedProps.Name} v{GeneratedProps.Version} is started!");
    }

    public BetterPlayerControl GetPlayerById(byte playerId)
    {
        return AllPlayers.Find(x => x.Player && x.Player.PlayerId == playerId);
    }
    
    public BetterPlayerControl GetPlayerByOwnerId(int ownerId)
    {
        return AllPlayers.Find(x => x.Player && x.Player.OwnerId == ownerId);
    }

    private static TType AttachManager<TType>() where TType : new()
    {
        return new TType();
    }

    private TComponent AttachComponent<TComponent>(AssetBundle bundle, string path) where TComponent : MonoBehaviour
    {
        return Instantiate(bundle.LoadComponent<TComponent>(path), transform);
    }

    private static void OnPlayerJoined(PlayerControl player)
    {
        player.gameObject.AddComponent<BetterPlayerControl>();
    }
}