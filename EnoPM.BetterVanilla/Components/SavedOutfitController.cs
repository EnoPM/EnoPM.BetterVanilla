using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Core.Data.Database;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class SavedOutfitController : MonoBehaviour
{
    public Button button;
    public Button deleteButton;
    public Image hatPreview;
    public Image skinPreview;
    public Image visorPreview;
    public Image petPreview;
    public Image nameplatePreview;

    internal DressingOutfit Outfit { get; set; }

    private void Awake()
    {
        deleteButton.onClick.AddListener((UnityAction)OnDeleteButtonClick);
    }

    private void Start()
    {
        button.onClick.AddListener((UnityAction)OnClick);

        this.StartCoroutine(CoLoadHat());
        this.StartCoroutine(CoLoadSkin());
        this.StartCoroutine(CoLoadVisor());
        this.StartCoroutine(CoLoadPet());
        this.StartCoroutine(CoLoadNameplate());
    }

    private void OnClick()
    {
        if (Utils.IsGameStarted) return;
        Outfit.ApplyOutfitToLocalPlayer();
        DressingOutfitTabController.Instance.RefreshSelectedOutfit();
    }

    private IEnumerator CoLoadHat()
    {
        var viewData = HatManager.Instance.GetHatById(Outfit.Hat);
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            hatPreview.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(hatPreview.gameObject, asset);
        }));
    }
    
    private IEnumerator CoLoadSkin()
    {
        var viewData = HatManager.Instance.GetSkinById(Outfit.Skin);
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            skinPreview.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(skinPreview.gameObject, asset);
        }));
    }
    
    private IEnumerator CoLoadVisor()
    {
        var viewData = HatManager.Instance.GetVisorById(Outfit.Visor);
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            visorPreview.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(visorPreview.gameObject, asset);
        }));
    }
    
    private IEnumerator CoLoadPet()
    {
        var viewData = HatManager.Instance.GetPetById(Outfit.Pet);
        viewData.CoLoadPreview(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            petPreview.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(petPreview.gameObject, asset);
        }));
        yield break;
    }
    
    private IEnumerator CoLoadNameplate()
    {
        var viewData = HatManager.Instance.GetNamePlateById(Outfit.Nameplate);
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            nameplatePreview.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(nameplatePreview.gameObject, asset);
        }));
    }

    private void OnDeleteButtonClick()
    {
        Outfit.Delete();
        ModMenuController.Instance.DressingOutfitTab.SavedOutfits.Remove(this);
        Destroy(gameObject);
    }
}