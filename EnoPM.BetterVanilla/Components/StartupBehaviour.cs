using System;
using System.Collections;
using System.Reflection;
using BepInEx.Unity.IL2CPP.Utils;
using Cpp2IL.Core.Extensions;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnoPM.BetterVanilla.Components;

public sealed class StartupBehaviour : MonoBehaviour
{
    private const string AssetBundleAssemblyResourcePath = "EnoPM.BetterVanilla.Resources.bv.ui";
    private const string UiUpdaterPrefabAssetBundlePath = "assets/bettervanilla/ui/updater/modupdater.prefab";
    private const string UiMenuPrefabAssetBundlePath = "assets/bettervanilla/ui/menu/modmenu.prefab";
    
    private void OnStarted(AssetBundle assetBundle)
    {
        DestroyableSingleton<ModManager>.Instance.ShowModStamp();
        
        CreateUpdater(assetBundle);
    }

    private void OnLogged(AssetBundle assetBundle)
    {
        DB.RefreshPlayerName();
        CreateMenu(assetBundle);
    }
    
    private static void CreateUpdater(AssetBundle assetBundle)
    {
        var asset = assetBundle.LoadAsset(UiUpdaterPrefabAssetBundlePath);
        
        var updaterGameObject = new GameObject("BetterVanillaUpdater")
        {
            layer = 4,
            hideFlags = HideFlags.HideAndDontSave
        };
        
        var assetGameObject = Instantiate(asset, updaterGameObject.transform).Cast<GameObject>();
        assetGameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(assetGameObject);
    }

    private static void CreateMenu(AssetBundle assetBundle)
    {
        var asset = assetBundle.LoadAsset(UiMenuPrefabAssetBundlePath);
        
        var updaterGameObject = new GameObject("BetterVanillaMenu")
        {
            layer = 4,
            hideFlags = HideFlags.HideAndDontSave
        };
        
        var assetGameObject = Instantiate(asset, updaterGameObject.transform).Cast<GameObject>();
        assetGameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(assetGameObject);
    }
    
    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private static bool ConditionToStart()
    {
        if (!DestroyableSingleton<ModManager>.InstanceExists)
        {
            return false;
        }

        if (!DestroyableSingleton<AccountManager>.InstanceExists)
        {
            return false;
        }

        return true;
    }

    private static bool ConditionToLogin()
    {
        var client = AmongUsClient.Instance.GetClient(AmongUsClient.Instance.ClientId);
        if (client == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(client.PlayerName))
        {
            return false;
        }

        return true;
    }

    private IEnumerator CoStart()
    {
        while (!ConditionToStart())
        {
            yield return new WaitForSeconds(1f);
        }
        
        var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(AssetBundleAssemblyResourcePath);
        if (resourceStream == null)
        {
            throw new Exception($"Unable to locate resource {AssetBundleAssemblyResourcePath}");
        }
        var assetBundle = AssetBundle.LoadFromMemory(resourceStream.ReadBytes());
        if (!assetBundle)
        {
            throw new Exception($"Unable to load asset bundle {AssetBundleAssemblyResourcePath}");
        }
        
        OnStarted(assetBundle);

        while (!ConditionToLogin())
        {
            yield return new WaitForSeconds(1f);
        }
        
        OnLogged(assetBundle);
        
        assetBundle.Unload(false);
    }
}