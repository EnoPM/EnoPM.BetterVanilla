using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class SliderSettingItem : SettingItem
{
    private const float SaveTimerInSeconds = 1f;
    
    public Slider slider;
    public TextMeshProUGUI valueText;
    public Button decrementButton;
    public Button incrementButton;

    private float _stepSize = 1f;
    private float _minValue = 1f;
    private float _maxValue = 100f;
    private string _prefix = string.Empty;
    private string _suffix = string.Empty;

    private float _previousValue;
    private float _saveTimer = SaveTimerInSeconds;
    private bool _hasUnsavedChanged;

    private void Awake()
    {
        slider.onValueChanged.AddListener((UnityAction<float>)OnSliderValueChanged);
        decrementButton.onClick.AddListener((UnityAction)OnDecrementButtonClick);
        incrementButton.onClick.AddListener((UnityAction)OnIncrementButtonClick);
        slider.minValue = _minValue;
        slider.maxValue = _maxValue;
        RefreshValueText();
    }

    private void FixedUpdate()
    {
        if (!_hasUnsavedChanged) return;
        _saveTimer -= Time.fixedDeltaTime;
        if (_saveTimer > 0f) return;
        TriggerValueChangedHook();
        _saveTimer = SaveTimerInSeconds;
        _hasUnsavedChanged = false;
    }

    private void OnDisable()
    {
        if (!_hasUnsavedChanged) return;
        TriggerValueChangedHook();
        _saveTimer = SaveTimerInSeconds;
        _hasUnsavedChanged = false;
    }

    private void RegisterSaveToQueue()
    {
        _hasUnsavedChanged = true;
        _saveTimer = SaveTimerInSeconds;
    }

    public void SetMinValue(float minValue)
    {
        _minValue = minValue;
        if (slider)
        {
            slider.minValue = _minValue;
        }
    }

    public void SetMaxValue(float maxValue)
    {
        _maxValue = maxValue;
        if (slider)
        {
            slider.maxValue = _maxValue;
        }
    }

    public void SetStepSize(float stepSize)
    {
        _stepSize = stepSize;
    }

    public void SetPrefix(string prefix)
    {
        _prefix = prefix;
        RefreshValueText();
    }

    public void SetSuffix(string suffix)
    {
        _suffix = suffix;
        RefreshValueText();
    }

    public void SetValue(float value, bool unverified = false)
    {
        var newValue = unverified ? value : Mathf.Round(value / _stepSize) * _stepSize;
        if (!unverified)
        {
            if (newValue > _maxValue)
            {
                newValue = _maxValue;
            }
            else if (newValue < _minValue)
            {
                newValue = _minValue;
            }
        }
        slider.Set(newValue, false);
        RefreshValueText();
        RefreshButtonsState();
        if (!Mathf.Approximately(_previousValue, newValue))
        {
            RegisterSaveToQueue();
        }
        _previousValue = newValue;
    }

    public float GetSettingValue()
    {
        var value = slider.value;
        return value;
    }
    
    private void RefreshValueText()
    {
        if (!valueText || !slider) return;
        valueText.SetText($"{_prefix}{(float)Math.Round(slider.value, 2)}{_suffix}");
    }

    public void RefreshButtonsState()
    {
        var value = slider.value;
        decrementButton.interactable = value > _minValue;
        incrementButton.interactable = value < _maxValue;
    }

    private void OnSliderValueChanged(float value)
    {
        SetValue(value);
    }

    private void OnDecrementButtonClick()
    {
        SetValue(slider.value - _stepSize);
    }

    private void OnIncrementButtonClick()
    {
        SetValue(slider.value + _stepSize);
    }
}