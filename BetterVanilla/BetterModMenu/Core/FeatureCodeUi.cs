using System;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class FeatureCodeUi : MonoBehaviour
{
    public TMP_InputField codeField = null!;
    public Button submitButton = null!;
    public AlertUi alert = null!;

    private void OnEnable()
    {
        alert.Hide();
    }

    public void OnSubmitButtonClicked()
    {
        if (FeatureCodeBehaviour.Instance == null || string.IsNullOrWhiteSpace(codeField.text)) return;
        var result = FeatureCodeBehaviour.Instance.ApplyCode(codeField.text);
        switch (result)
        {
            case FeatureCodeResult.Enabled:
                alert.ShowSuccess("This feature code has been successfully activated.");
                codeField.SetText(string.Empty);
                break;
            case FeatureCodeResult.Disabled:
                alert.ShowWarning("This feature code has been successfully disabled.");
                codeField.SetText(string.Empty);
                break;
            case FeatureCodeResult.Unauthorized:
                alert.ShowError("You are not authorized to activate this feature code.");
                codeField.SetText(string.Empty);
                break;
            case FeatureCodeResult.Invalid:
                default:
                alert.ShowError("This feature code does not exist.");
                break;
        }
    }
}