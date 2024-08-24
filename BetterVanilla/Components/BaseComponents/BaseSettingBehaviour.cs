using BetterVanilla.Core.Options;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Components.BaseComponents;

public abstract class BaseSettingBehaviour : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    
    public BaseLocalOption Option { get; private set; }

    public virtual void Initialize(BaseLocalOption option)
    {
        Option = option;
    }

    protected virtual void Start()
    {
        nameText.SetText(Option.Title);
        UpdateFromOption();
        Option.OnBehaviourCreated(this);
    }

    public abstract void UpdateFromOption();
}