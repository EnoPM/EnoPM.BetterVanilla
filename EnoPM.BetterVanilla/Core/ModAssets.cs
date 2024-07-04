using System.Reflection;
using Cpp2IL.Core.Extensions;
using EnoPM.BetterVanilla.ManagedComponents;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

internal static class ModAssets
{
    internal static readonly Prefab<PopupController> ErrorPopupPrefab;
    internal static readonly Prefab<PopupController> SuccessPopupPrefab;
    internal static readonly Prefab<UpdaterController> UpdaterPrefab;

    static ModAssets()
    {
        var uiAssetBundle = AssetBundle.LoadFromMemory(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("EnoPM.BetterVanilla.Resources.bettervanilla.ui")!.ReadBytes());

        ErrorPopupPrefab = new Prefab<PopupController>(uiAssetBundle.LoadPrefab("ErrorPopup"));
        SuccessPopupPrefab = new Prefab<PopupController>(uiAssetBundle.LoadPrefab("SuccessPopup"));
        UpdaterPrefab = new Prefab<UpdaterController>(uiAssetBundle.LoadPrefab("UpdaterUi"));
        
        uiAssetBundle.Unload(false);
    }

    private static Object LoadPrefab(this AssetBundle bundle, string name) => bundle.LoadAsset($"assets/prefabs/{name.ToLowerInvariant()}.prefab");
    
    internal class Prefab<T> where T : MonoBehaviour
    {
        private readonly Object _asset;
        
        internal Prefab(Object asset)
        {
            _asset = asset;
        }

        internal T Instantiate(Transform parent) => Object.Instantiate(_asset, parent).Cast<GameObject>().GetComponent<T>();
    }
}