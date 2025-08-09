using UnityEngine;

namespace BetterVanilla.Components;

public sealed class ZoomBehaviourManager : MonoBehaviour
{
    private const float MinOrthographicSize = 3f;
    private const float MaxOrthographicSize = 12f;
    
    private Camera MainCamera { get; set; }
    private float CameraOrthographicSize { get; set; }
    private float DefaultCameraOrthographicSize { get; set; }
    private float CachedCameraOrthographicSize { get; set; }

    private bool _isMeeting;
    private bool IsMeeting
    {
        get => _isMeeting;
        set
        {
            if (_isMeeting == value) return;
            _isMeeting = value;
            if (_isMeeting)
            {
                OnMeetingStarted();
            }
            else
            {
                OnMeetingEnded();
            }
        }
    }
    
    private void Start()
    {
        MainCamera = Camera.main;
        if (MainCamera)
        {
            CameraOrthographicSize = DefaultCameraOrthographicSize = MainCamera.orthographicSize;
        }
        BetterVanillaManager.Instance.ZoomBehaviour = this;
    }

    private void OnDestroy()
    {
        BetterVanillaManager.Instance.ZoomBehaviour = null;
    }

    private void Update()
    {
        IsMeeting = MeetingHud.Instance;
        if (MainCamera)
        {
            var orthographicSize = MainCamera.orthographicSize;
            if (!Mathf.Approximately(orthographicSize, CameraOrthographicSize))
            {
                UpdateOrthographicSize();
            }
        }
    }

    private void UpdateOrthographicSize()
    {
        if (MainCamera == null) return;
        MainCamera.orthographicSize = CameraOrthographicSize;
        foreach (var camera in Camera.allCameras)
        {
            if (camera == null || camera.gameObject == null || (camera.name != "UI Camera" && camera.name != "KeyMapper Camera")) continue;
            camera.orthographicSize = CameraOrthographicSize;
        }
        HudManager.Instance.Chat.chatButtonAspectPosition.AdjustPosition();
    }

    private void OnMeetingStarted()
    {
        CachedCameraOrthographicSize = CameraOrthographicSize;
        CameraOrthographicSize = DefaultCameraOrthographicSize;
    }

    private void OnMeetingEnded()
    {
        CameraOrthographicSize = CachedCameraOrthographicSize;
    }

    public bool CanZoomOut(float increment)
    {
        if (IsMeeting)
        {
            return CachedCameraOrthographicSize + increment <= MaxOrthographicSize;
        }
        return CameraOrthographicSize + increment <= MaxOrthographicSize;
    }

    public void ZoomOut(float increment)
    {
        if (!CanZoomOut(increment)) return;
        if (IsMeeting)
        {
            CachedCameraOrthographicSize = Mathf.Clamp(CachedCameraOrthographicSize + increment, MinOrthographicSize, MaxOrthographicSize);
        }
        else
        {
            CameraOrthographicSize = Mathf.Clamp(CameraOrthographicSize + increment, MinOrthographicSize, MaxOrthographicSize);
        }
    }

    public bool CanZoomIn(float decrement)
    {
        if (IsMeeting)
        {
            return CachedCameraOrthographicSize - decrement >= MinOrthographicSize;
        }
        return CameraOrthographicSize - decrement >= MinOrthographicSize;
    }
    
    public void ZoomIn(float increment)
    {
        if (!CanZoomIn(increment)) return;
        if (IsMeeting)
        {
            CachedCameraOrthographicSize = Mathf.Clamp(CachedCameraOrthographicSize - increment, MinOrthographicSize, MaxOrthographicSize);
        }
        else
        {
            CameraOrthographicSize = Mathf.Clamp(CameraOrthographicSize - increment, MinOrthographicSize, MaxOrthographicSize);
        }
    }

    public float GetZoomValue()
    {
        return IsMeeting ? CachedCameraOrthographicSize : CameraOrthographicSize;
    }
}