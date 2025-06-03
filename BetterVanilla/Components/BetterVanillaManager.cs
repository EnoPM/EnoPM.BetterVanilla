using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.GeneratedRuntime;
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
    public ZoomBehaviourManager ZoomBehaviour { get; internal set; }
    public TaskFinisherBehaviour TaskFinisher { get; internal set; }
    public Sprite VentSprite { get; private set; }
    
    private void Awake()
    {
        if (Instance) throw new Exception($"{nameof(BetterVanillaManager)} must be a singleton");
        Instance = this;
        
        Database = new DatabaseManager();
        Features = new FeaturesManager();
        ChatCommands = new ChatCommandsManager();
        HostOptions = new HostOptionsHolder();
        LocalOptions = new LocalOptionsHolder();
        Cheaters = new CheatersManager();
        Xp = new XpManager();

        var uiBundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.ui");

        MenuButton = Instantiate(uiBundle.LoadComponent<ModMenuButton>("Assets/Ui/Components/ModMenuButton.prefab"), transform);
        Updater = Instantiate(uiBundle.LoadComponent<ModUpdater>("Assets/Ui/Windows/ModUpdaterUi.prefab"), transform);
        Menu = Instantiate(uiBundle.LoadComponent<ModMenu>("Assets/Ui/Windows/ModMenuUi.prefab"), transform);
        
        uiBundle.Unload(false);
        
        var gameBundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.game");
        
        VentSprite = gameBundle.LoadAsset<Sprite>("Assets/Sprites/Vent.png");
        VentSprite.hideFlags = HideFlags.HideAndDontSave;
        
        gameBundle.Unload(false);

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
        Features.Initialize("EnoPM/EnoPM.BetterVanilla", "master", "features-registry.json");
        
        Ls.LogInfo($"Plugin {GeneratedProps.Name} v{GeneratedProps.Version} was successfully started!");
    }

    public BetterPlayerControl GetPlayerById(byte playerId)
    {
        return AllPlayers.Find(x => x.Player && x.Player.PlayerId == playerId);
    }
    
    public BetterPlayerControl GetPlayerByOwnerId(int ownerId)
    {
        return AllPlayers.Find(x => x.Player && x.Player.OwnerId == ownerId);
    }

    private static void OnPlayerJoined(PlayerControl player)
    {
        player.gameObject.AddComponent<BetterPlayerControl>();
    }
}