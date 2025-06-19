using UnityEngine.UI;
using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class NumberOptionUi : BaseOptionUi
{
    public Slider slider = null!;
    public Button minusButton = null!;
    public Button plusButton = null!;
    public TextMeshProUGUI valueText = null!;
}