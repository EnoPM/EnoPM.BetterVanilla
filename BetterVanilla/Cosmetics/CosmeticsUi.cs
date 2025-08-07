using System;
using System.Collections;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core;
using Innersloth.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Cosmetics;

public sealed class CosmeticsUi : MonoBehaviour
{
    public GameObject canvas;
    public Button closeButton;
    public Image playerPreviewImage;
    public PlayerPreviewUi playerPreview;
    public ColorPickerUi colorPicker;
    
    private Coroutine LoadPreviewCoroutine { get; set; }
    
    private readonly UiInteractionBlocker _blocker = new();

    private void Awake()
    {
        closeButton.onClick.AddListener(new Action(Close));
        colorPicker.DeferredUpdate += OnColorPickerDeferredUpdate;
        Close();
    }

    private void OnColorPickerDeferredUpdate(Color color)
    {
        if (PlayerControl.LocalPlayer == null) return;
        var control = PlayerControl.LocalPlayer.gameObject.GetComponent<BetterPlayerControl>();
        if (control == null) return;
        control.SetVisorColor(color);
    }

    public void Open()
    {
        canvas.SetActive(true);
    }

    public void Close()
    {
        canvas.SetActive(false);
    }

    private IEnumerator CoLoadPreview()
    {
        while (!HatManager.InstanceExists || !DataManager.IsPlayerLoaded || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.cosmetics == null)
        {
            yield return new WaitForSeconds(1f);
        }
        Ls.LogMessage($"Loading preview for {PlayerControl.LocalPlayer.Data.PlayerName}");
        var material = HatManager.Instance.PlayerMaterial;
        PlayerMaterial.SetColors(DataManager.Player.Customization.Color, material);
        playerPreviewImage.material = material;
        playerPreviewImage.sprite = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.sprite;
    }
    
    private static IEnumerator CoLoadCosmeticData(Image image, CosmeticData viewData)
    {
        yield return viewData.CoLoadIcon(new Action<Sprite, AddressableAsset>((sprite, asset) =>
        {
            image.sprite = sprite;
            AddressableAssetHandler.AddToGameObject(image.gameObject, asset);
        }));
    }

    private void OnEnable()
    {
        _blocker.Block();
        if (LoadPreviewCoroutine == null)
        {
            LoadPreviewCoroutine = this.StartCoroutine(CoLoadPreview());
        }
    }

    private void OnDisable()
    {
        _blocker.Unblock();
        if (LoadPreviewCoroutine != null)
        {
            StopCoroutine(LoadPreviewCoroutine);
            LoadPreviewCoroutine = null;
        }
    }
}