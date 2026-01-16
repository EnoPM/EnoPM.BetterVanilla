using System.Collections;
using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using UnityEngine;
using ILogger = Hazel.ILogger;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(TutorialManager))]
internal static class TutorialManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(TutorialManager.Awake))]
    private static bool AwakePrefix(TutorialManager __instance)
    {
        __instance.BaseAwake();
        DataManager.Player.Stats.SetStatTrackingEnabled(false);
        __instance.StartCoroutine(__instance.CoRunTutorial());
        GameDebugCommands.AddCommands();
        TutorialDebugCommands.AddCommands(__instance.gameObject);
        return false;
    }

    private static IEnumerator CoRunTutorial(this TutorialManager tutorialManager)
    {
        while (ShipStatus.Instance == null) yield return null;

        ShipStatus.Instance.Timer = 15f;

        while (PlayerControl.LocalPlayer == null) yield return null;
        while (GameManager.Instance == null) yield return null;

        if (DiscordManager.InstanceExists)
        {
            DiscordManager.Instance.SetHowToPlay();
        }

        var options = new NormalGameOptionsV10(new UnityLogger().As<ILogger>());
        options.SetInt(Int32OptionNames.NumImpostors, 0);
        options.DiscussionTime = 0;
        options.NumEmergencyMeetings = 9;
        options.GhostsDoTasks = true;
        GameOptionsManager.Instance.CurrentGameOptions = options.Cast<IGameOptions>();
        PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
        PlayerControl.LocalPlayer.AdjustLighting();
        PlayerControl.LocalPlayer.cosmetics.SetAsLocalPlayer();

        for (var i = 0; i < ShipStatus.Instance.DummyLocations.Length; i++)
        {
            var player = Object.Instantiate(tutorialManager.PlayerPrefab);
            player.PlayerId = (byte)GameData.Instance.GetAvailableId();
            var data = GameData.Instance.AddDummy(player);
            AmongUsClient.Instance.Spawn(data);
            AmongUsClient.Instance.Spawn(player);
            player.isDummy = true;
            player.transform.position = ShipStatus.Instance.DummyLocations[i].position;
            player.GetComponent<DummyBehaviour>().enabled = true;
            player.NetTransform.enabled = false;
            player.SetDummyCosmetics(i);
            data.SetDummyTasks();
        }
        yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
        ShipStatus.Instance.Begin();
        GameManager.Instance.StartGame();
        ShipStatus.Instance.StartSFX();
        
        Ls.LogInfo($"Started Freeplay Game in {(MapNames) AmongUsClient.Instance.TutorialMapId}");
    }

    private static void SetDummyCosmetics(this PlayerControl player, int i)
    {
        player.SetName($"{TranslationController.Instance.GetString(StringNames.Dummy)} {i}");
        var colorId = i < DataManager.player.Customization.Color ? i : i + 1;
        player.SetColor(colorId);
        player.SetHat(HatManager.Instance.allHats.GetRandomProductId(), colorId);
        player.SetSkin(HatManager.Instance.allSkins.GetRandomProductId(), colorId);
        player.SetPet(HatManager.Instance.allPets.GetRandomProductId());
        player.SetVisor(HatManager.Instance.allVisors.GetRandomProductId(), colorId);
        player.SetNamePlate(HatManager.Instance.allNamePlates.GetRandomProductId());
        player.SetLevel(uint.MinValue);
    }

    private static string GetRandomProductId<T>(this Il2CppReferenceArray<T> items) where T : CosmeticData
    {
        return items.Length == 0 ? string.Empty : items[Random.Range(0, items.Length)].ProductId;
    }

    private static void SetDummyTasks(this NetworkedPlayerInfo data)
    {
        data.RpcSetTasks(new Il2CppStructArray<byte>(0));
    }
}