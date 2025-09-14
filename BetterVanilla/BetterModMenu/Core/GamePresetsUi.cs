using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Components.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class GamePresetsUi : MonoBehaviour
{
    public LockOverlayUi globalLockOverlay = null!;
    public TMP_InputField presetNameField = null!;
    public Button savePresetButton = null!;
    public LockOverlayUi lockOverlay = null!;
    public TMP_Dropdown presetDropdown = null!;
    public Button applyPresetButton = null!;
    public Button deletePresetButton = null!;

    private List<SerializableGamePreset> Presets { get; } = [];

    private void Awake()
    {
        presetDropdown.onValueChanged.AddListener(new Action<int>(OnPresetDropdownValueChanged));
    }

    private void OnPresetDropdownValueChanged(int _)
    {
        if (!string.IsNullOrWhiteSpace(presetNameField.text)) return;
        var presetName = GetSelectedPresetName();
        if (presetName == null) return;
        presetNameField.SetText(presetName);
    }

    private void Start()
    {
        presetDropdown.ClearOptions();
        Presets.AddRange(LoadPresets());
        ResetDropdown();
    }

    private void ResetDropdown()
    {
        var presetName = GetSelectedPresetName();
        presetDropdown.ClearOptions();
        var presetNames = new List<string>();
        foreach (var preset in Presets)
        {
            presetNames.Add(preset.Name);
        }
        presetNames.Sort(StringComparer.CurrentCulture);
        presetDropdown.AddOptions(presetNames.ToIl2CppList());
        var presetIndex = presetNames.FindIndex(x => x == presetName);
        if (presetIndex < 0 || presetIndex >= Presets.Count) return;
        presetDropdown.value = presetIndex;
    }

    private List<SerializableGamePreset> LoadPresets()
    {
        var presets = new List<SerializableGamePreset>();
        if (!File.Exists(ModPaths.GamePresetsFile)) return presets;
        using var file = File.OpenRead(ModPaths.GamePresetsFile);
        using var reader = new BinaryReader(file);
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            try
            {
                var preset = new SerializableGamePreset(reader);
                presets.Add(preset);
            }
            catch (Exception ex)
            {
                Ls.LogWarning($"Old preset version, skipping loading: {ex.Message}");
            }
        }
        return presets;
    }

    private void SavePresets()
    {
        using var file = File.Create(ModPaths.GamePresetsFile);
        using var writer = new BinaryWriter(file);
        writer.Write(Presets.Count);
        foreach (var preset in Presets)
        {
            preset.Serialize(writer);
        }
    }

    public void OnSavePresetClicked()
    {
        var presetName = presetNameField.text;
        if (string.IsNullOrWhiteSpace(presetName)) return;
        var preset = Presets.FirstOrDefault(x => x.Name == presetName);
        if (preset != null)
        {
            Presets.Remove(preset);
        }
        preset = new SerializableGamePreset(presetName);
        Presets.Add(preset);
        SavePresets();
        ResetDropdown();
        presetNameField.SetText(string.Empty);
    }

    public void OnApplyPresetClicked()
    {
        var preset = GetSelectedPreset();
        preset?.Apply();
    }

    public void OnDeletePresetClicked()
    {
        var preset = GetSelectedPreset();
        if (preset == null) return;
        Presets.Remove(preset);
        SavePresets();
        ResetDropdown();
    }

    private SerializableGamePreset? GetSelectedPreset()
    {
        var presetName = GetSelectedPresetName();
        if (presetName == null) return null;
        return Presets.FirstOrDefault(x => x.Name == presetName);
    }

    private string? GetSelectedPresetName()
    {
        var presetNames = Presets.Select(x => x.Name).ToList();
        var index = presetDropdown.value;
        if (index < 0 || index >= presetNames.Count) return null;
        return presetNames[index];
    }

    private void Update()
    {
        if (lockOverlay == null) return;
        if (LocalConditions.IsGameStarted())
        {
            lockOverlay.SetLockedText("Available in lobby");
            lockOverlay.SetActive(true);
            return;
        }
        lockOverlay.SetLockedText("Available for host only");
        lockOverlay.SetActive(!LocalConditions.AmHost());
    }
}