using System;
using System.Collections;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Cosmetics;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class SavedOutfitItemUi : MonoBehaviour
{
    public Button applyButton = null!;
    public Button deleteButton = null!;
    public Image hat = null!;
    public Image skin = null!;
    public Image visor = null!;
    public Image pet = null!;
    public Image nameplate = null!;
    
    public SerializableOutfit? Outfit { get; set; }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private void Update()
    {
        applyButton.interactable = Outfit != null && !Outfit.IsEquipped();
    }

    private IEnumerator CoStart()
    {
        while (Outfit == null)
        {
            yield return null;
        }
        
        while (HatManager.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (Outfit.Hat != string.Empty)
        {
            yield return this.StartCoroutine(CoLoadHat(hat, HatManager.Instance.GetHatById(Outfit.Hat)));
        }
        else
        {
            hat.enabled = false;
        }

        if (Outfit.Skin != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(skin, HatManager.Instance.GetSkinById(Outfit.Skin)));
        }
        else
        {
            skin.enabled = false;
        }

        if (Outfit.Visor != string.Empty)
        {
            this.StartCoroutine(CoLoadVisor(visor, HatManager.Instance.GetVisorById(Outfit.Visor)));
        }
        else
        {
            visor.enabled = false;
        }

        if (Outfit.Nameplate != string.Empty)
        {
            this.StartCoroutine(CoLoadCosmeticData(nameplate, HatManager.Instance.GetNamePlateById(Outfit.Nameplate)));
        }
        else
        {
            nameplate.enabled = false;
        }

        if (Outfit.Pet != string.Empty)
        {
            this.StartCoroutine(CoLoadPet());
        }
        else
        {
            pet.enabled = false;
        }
    }

    private static IEnumerator CoLoadHat(Image image, HatData hat)
    {
        if (CosmeticsManager.Hats.TryGetCosmetic(hat.ProductId, out var cosmetic))
        {
            image.sprite = cosmetic.PreviewResource;
            yield break;
        }
        yield return CoLoadCosmeticData(image, hat);
    }
    
    private static IEnumerator CoLoadVisor(Image image, VisorData visor)
    {
        if (CosmeticsManager.Visors.TryGetCosmetic(visor.ProductId, out var cosmetic))
        {
            image.sprite = cosmetic.PreviewResource;
            yield break;
        }
        yield return CoLoadCosmeticData(image, visor);
    }

    private static IEnumerator CoLoadCosmeticData(Image image, CosmeticData viewData)
    {
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            image.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(image.gameObject, asset);
        }));
    }
    
    private IEnumerator CoLoadPet()
    {
        while (Outfit == null)
        {
            yield return null;
        }
        var viewData = HatManager.Instance.GetPetById(Outfit.Pet);
        viewData.CoLoadPreview(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            pet.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(pet.gameObject, asset);
        }));
    }

    public void OnApplyButtonClicked()
    {
        if (LocalConditions.IsGameStarted() || Outfit == null) return;
        var player = PlayerControl.LocalPlayer;
        if (player == null || player.Data == null) return;
        DataManager.Player.Customization.Hat = Outfit.Hat;
        player.RpcSetHat(Outfit.Hat);
        DataManager.Player.Customization.Skin = Outfit.Skin;
        player.RpcSetSkin(Outfit.Skin);
        DataManager.Player.Customization.Visor = Outfit.Visor;
        player.RpcSetVisor(Outfit.Visor);
        DataManager.Player.Customization.Pet = Outfit.Pet;
        player.RpcSetPet(Outfit.Pet);
        DataManager.Player.Customization.NamePlate = Outfit.Nameplate;
        player.RpcSetNamePlate(Outfit.Nameplate);
        DataManager.Player.Save();
    }

    public void OnDeleteButtonClicked()
    {
        if (Outfit == null) return;
        BetterVanillaManager.Instance.Menu.Ui.outfitSaverTab.DeleteOutfit(Outfit);
    }

    public void SetApplyButtonEnabled(bool state)
    {
        applyButton.interactable = state;
    }
}