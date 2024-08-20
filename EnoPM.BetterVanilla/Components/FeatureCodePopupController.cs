using EnoPM.BetterVanilla.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class FeatureCodePopupController : MonoBehaviour
{
    public Button closeButton;
    public TMP_InputField codeField;
    public Button submitButton;

    private void Awake()
    {
        closeButton.onClick.AddListener((UnityAction)Close);
        submitButton.onClick.AddListener((UnityAction)OnSubmitButtonClick);
    }

    private void OnSubmitButtonClick()
    {
        if (string.IsNullOrWhiteSpace(codeField.text)) return;
        FeatureLocker.RegisterCode(codeField.text);
        Close();
    }

    private void Close()
    {
        Destroy(gameObject);
    }
}