using System;
using System.Collections;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu.Outfits;

public sealed class SavedOutfitItem : MonoBehaviour
{
    public Button itemButton;
    public Button deleteButton;
    public Image hatImage;
    public Image skinImage;
    public Image visorImage;
    public Image petImage;
    public Image nameplateImage;
    
    public LocalOutfitData OutfitData { get; private set; }

    public void Initialize(LocalOutfitData outfitData)
    {
        OutfitData = outfitData;
    }

    private void Awake()
    {
        itemButton.onClick.AddListener(new Action(OnClick));
        deleteButton.onClick.AddListener(new Action(OnDeleteButtonClick));
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        while (!HatManager.Instance)
        {
            yield return new WaitForEndOfFrame();
        }
        if (OutfitData.Hat != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(hatImage, HatManager.Instance.GetHatById(OutfitData.Hat)));
        }
        else
        {
            hatImage.enabled = false;
        }

        if (OutfitData.Skin != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(skinImage, HatManager.Instance.GetSkinById(OutfitData.Skin)));
        }
        else
        {
            skinImage.enabled = false;
        }

        if (OutfitData.Visor != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(visorImage, HatManager.Instance.GetVisorById(OutfitData.Visor)));
        }
        else
        {
            visorImage.enabled = false;
        }

        if (OutfitData.Nameplate != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(nameplateImage, HatManager.Instance.GetNamePlateById(OutfitData.Nameplate)));
        }
        else
        {
            nameplateImage.enabled = false;
        }

        if (OutfitData.Pet != string.Empty)
        {
            this.StartCoroutine(CoLoadPet());
        }
        else
        {
            petImage.enabled = false;
        }
        
    }

    private void OnDeleteButtonClick()
    {
        BetterVanillaManager.Instance.Database.Data.Outfits.Remove(OutfitData);
        BetterVanillaManager.Instance.Database.Save();
        BetterVanillaManager.Instance.Menu.OutfitsTab.AllSavedOutfits.Remove(this);
        Destroy(gameObject);
    }

    private void OnClick()
    {
        if (LocalConditions.IsGameStarted()) return;
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return;
        DataManager.Player.Customization.Hat = OutfitData.Hat;
        player.RpcSetHat(OutfitData.Hat);
        DataManager.Player.Customization.Skin = OutfitData.Skin;
        player.RpcSetSkin(OutfitData.Skin);
        DataManager.Player.Customization.Visor = OutfitData.Visor;
        player.RpcSetVisor(OutfitData.Visor);
        DataManager.Player.Customization.Pet = OutfitData.Pet;
        player.RpcSetPet(OutfitData.Pet);
        DataManager.Player.Customization.NamePlate = OutfitData.Nameplate;
        player.RpcSetNamePlate(OutfitData.Nameplate);
        DataManager.Player.Save();
        BetterVanillaManager.Instance.Menu.OutfitsTab.RefreshSelectedOutfit();
    }

    private IEnumerator CoLoadCosmeticData(Image image, CosmeticData viewData)
    {
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            image.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(image.gameObject, asset);
        }));
    }
    
    private IEnumerator CoLoadPet()
    {
        var viewData = HatManager.Instance.GetPetById(OutfitData.Pet);
        viewData.CoLoadPreview(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            petImage.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(petImage.gameObject, asset);
        }));
        yield break;
    }
}