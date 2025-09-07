using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.BetterModMenu;
using BetterVanilla.GeneratedRuntime;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterVanillaManager : MonoBehaviour
{
    public static BetterVanillaManager Instance { get; private set; } = null!;

    private readonly Harmony _harmony = new (GeneratedProps.Guid);
    public readonly List<BetterPlayerControl> AllPlayers = [];
    public readonly Dictionary<int, TeamPreferences> AllTeamPreferences = [];
    public readonly Dictionary<int, TeamPreferences> AllForcedTeamAssignments = [];
    public ChatCommandsManager ChatCommands { get; private set; } = null!;
    public CheatersManager Cheaters { get; private set; } = null!;
    public XpManager Xp { get; private set; } = null!;
    public ModMenu Menu { get; private set; } = null!;
    public ZoomBehaviourManager ZoomBehaviour { get; internal set; } = null!;
    public Sprite VentSprite { get; private set; } = null!;
    public BetterPlayerTexts PlayerTextsPrefab { get; private set; } = null!;
    public BetterPlayerTexts BetterVoteAreaTextsPrefab { get; private set; } = null!;
    
    private void Awake()
    {
        if (Instance) throw new Exception($"{nameof(BetterVanillaManager)} must be a singleton");
        Instance = this;
        
        ChatCommands = new ChatCommandsManager();
        Cheaters = new CheatersManager();
        Xp = new XpManager();
        Menu = new ModMenu();
        
        var gameBundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.game");
        
        VentSprite = gameBundle.LoadAsset<Sprite>("Assets/Sprites/Vent.png");
        VentSprite.hideFlags = HideFlags.HideAndDontSave;
        
        gameBundle.Unload(false);
        
        var betterGame = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.better.game");
        
        PlayerTextsPrefab = Instantiate(betterGame.LoadComponent<BetterPlayerTexts>("Assets/Ui/BetterPlayerTexts.prefab"), transform);
        PlayerTextsPrefab.gameObject.SetActive(false);
        
        BetterVoteAreaTextsPrefab = Instantiate(betterGame.LoadComponent<BetterPlayerTexts>("Assets/Ui/BetterVoteAreaTexts.prefab"), transform);
        BetterVoteAreaTextsPrefab.gameObject.SetActive(false);
        
        betterGame.Unload(false);

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
        yield return new WaitForSeconds(5f);
        while (!ModManager.InstanceExists)
        {
            yield return new WaitForSeconds(1f);
        }
        
        ModManager.Instance.ShowModStamp();
        
        Ls.LogInfo($"Plugin {GeneratedProps.Name} v{GeneratedProps.Version} was successfully started!");
    }

    public BetterPlayerControl? GetPlayerById(byte playerId)
    {
        return AllPlayers.Find(x => x.Player != null && x.Player.PlayerId == playerId);
    }
    
    public BetterPlayerControl? GetPlayerByOwnerId(int ownerId)
    {
        return AllPlayers.Find(x => x.Player != null && x.Player.OwnerId == ownerId);
    }

    public BetterPlayerControl? GetPlayerByFriendCode(string friendCode)
    {
        return AllPlayers.Find(x => x.FriendCode != null && x.FriendCode == friendCode);
    }

    private static void OnPlayerJoined(PlayerControl player)
    {
        player.gameObject.AddComponent<BetterPlayerControl>();
    }
}