using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.ManagedComponents;

public class PopupController : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button closeButton;
    public Button confirmButton;

    private void Start()
    {
        SetActive(false);
        closeButton.onClick.AddListener((UnityAction)Close);
        confirmButton.onClick.AddListener((UnityAction)Close);
    }

    public void SetActive(bool value) => canvas.SetActive(value);
    
    public IEnumerator CoShow(string title, string body)
    {
        titleText.SetText(title);
        bodyText.SetText(body);

        SetActive(true);
        
        while (canvas.active)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public void Close() => SetActive(false);
}