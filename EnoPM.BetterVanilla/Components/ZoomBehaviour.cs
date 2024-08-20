using EnoPM.BetterVanilla.Core;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class ZoomBehaviour : MonoBehaviour
{
    internal static ZoomBehaviour Instance { get; private set; }
    private Camera MainCamera { get; set; }
    private float MinOrthographicSize { get; set; }
    private float MaxOrthographicSize { get; set; }
    private float StepOrthographicSize { get; set; }
    private float CameraOrthographicSize { get; set; }

    private void Awake()
    {
        Instance = this;
        MinOrthographicSize = 3f;
        MaxOrthographicSize = 12f;
        StepOrthographicSize = 1f;
        CameraOrthographicSize = 3f;
    }

    private void Start()
    {
        MainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Utils.IsGameStarted || !Utils.AmDead || Utils.AmImpostor) return;

        var value = (float)ModSettings.Local.ZoomValueOnDeath;
        if (!Mathf.Approximately(value, CameraOrthographicSize))
        {
            SetZoom(value);
            UpdateZoom();
        }
    }

    private void ZoomIn()
    {
        SetZoom(CameraOrthographicSize - StepOrthographicSize);
        UpdateZoom();
    }

    private void ZoomOut()
    {
        SetZoom(CameraOrthographicSize + StepOrthographicSize);
        UpdateZoom();
    }

    internal void ResetZoom() => SetZoom(MinOrthographicSize);

    public void SetZoom(float value)
    {
        CameraOrthographicSize = value;
        if (CameraOrthographicSize > MaxOrthographicSize)
        {
            CameraOrthographicSize = MaxOrthographicSize;
        }
        else if (CameraOrthographicSize < MinOrthographicSize)
        {
            CameraOrthographicSize = MinOrthographicSize;
        }
    }

    private void UpdateZoom()
    {
        if (!MainCamera) return;
        MainCamera.orthographicSize = CameraOrthographicSize;
        foreach (var camera in Camera.allCameras)
        {
            Plugin.Logger.LogMessage($"Camera: {camera.name} {camera.gameObject.name}");
            if (!camera || !camera.gameObject || (camera.name != "UI Camera" && camera.name != "KeyMapper Camera")) continue;
            camera.orthographicSize = CameraOrthographicSize;
        }
        HudManager.Instance.Chat.chatButtonAspectPosition.AdjustPosition();
    }
}