using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Config;
using AmongUsCosmeticsManager.Services;
using AmongUsCosmeticsManager.Services.BundleImport;

namespace AmongUsCosmeticsManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ProjectService _projectService;

    [ObservableProperty]
    private CosmeticBundle? _selectedBundle;

    [ObservableProperty]
    private CosmeticItem? _selectedItem;

    [ObservableProperty]
    private string _workspacePath = string.Empty;

    // Modal state
    [ObservableProperty]
    private bool _isModalOpen;

    [ObservableProperty]
    private string _modalTitle = string.Empty;

    [ObservableProperty]
    private string _modalInput = string.Empty;

    [ObservableProperty]
    private string _modalConfirmText = "OK";

    [ObservableProperty]
    private ModalMode _modalMode;

    [ObservableProperty]
    private PlayerColor _selectedPlayerColor = PlayerPalette.Colors[0];

    [ObservableProperty]
    private byte[]? _recoloredPlayerTemplate;

    public AppConfig AppConfig { get; }
    public ObservableCollection<CosmeticBundle> Bundles { get; } = [];
    public bool HasProjectFile => _projectService.HasProject;
    public PlayerColor[] PlayerColors => PlayerPalette.Colors;

    public MainViewModel() : this(new ConfigService(), new ProjectService()) { }

    private byte[]? _playerTemplateRaw;

    public MainViewModel(ConfigService configService, ProjectService projectService)
    {
        _projectService = projectService;
        configService.Load();
        AppConfig = configService.AppConfig;
        LoadPlayerTemplate();
        UpdateRecoloredTemplate();
    }

    private void LoadPlayerTemplate()
    {
        var assembly = typeof(MainViewModel).Assembly;
        using var stream = assembly.GetManifestResourceStream("AmongUsCosmeticsManager.Assets.player-template.png");
        if (stream == null) return;
        using var ms = new System.IO.MemoryStream();
        stream.CopyTo(ms);
        _playerTemplateRaw = ms.ToArray();
    }

    private void UpdateRecoloredTemplate()
    {
        if (_playerTemplateRaw == null) return;
        // Red zone = body front, Blue zone = body back (shadow), Green zone = visor (keep default teal)
        RecoloredPlayerTemplate = RecolorService.Recolor(
            _playerTemplateRaw,
            SelectedPlayerColor.Body,
            new SkiaSharp.SKColor(149, 202, 220), // VisorColor from Palette.cs
            SelectedPlayerColor.Shadow);
    }

    partial void OnSelectedPlayerColorChanged(PlayerColor value)
    {
        UpdateRecoloredTemplate();
    }

    public void OpenProject(string filePath)
    {
        _projectService.SetProjectFile(filePath);
        WorkspacePath = filePath;

        Bundles.Clear();
        SelectedBundle = null;
        SelectedItem = null;

        var loaded = _projectService.Load();
        foreach (var bundle in loaded)
            Bundles.Add(bundle);

        if (Bundles.Count > 0)
            SelectedBundle = Bundles[0];
    }

    public void SetProjectFile(string filePath)
    {
        _projectService.SetProjectFile(filePath);
        WorkspacePath = filePath;
    }

    [RelayCommand]
    private void SaveProject()
    {
        _projectService.Save(Bundles);
    }

    public void ImportBundle(string filePath)
    {
        var bundle = BundleImporter.Import(filePath);
        Bundles.Add(bundle);
        SelectedBundle = bundle;
    }

    // === Modal ===

    [RelayCommand]
    private void ShowAddBundleModal()
    {
        ModalTitle = "Nouveau Bundle";
        ModalInput = string.Empty;
        ModalConfirmText = "Creer";
        ModalMode = ModalMode.AddBundle;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void ShowRenameBundleModal()
    {
        if (SelectedBundle == null) return;
        ModalTitle = "Renommer le Bundle";
        ModalInput = SelectedBundle.Name;
        ModalConfirmText = "Renommer";
        ModalMode = ModalMode.RenameBundle;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void ConfirmModal()
    {
        var input = ModalInput.Trim();
        if (string.IsNullOrEmpty(input)) { CancelModal(); return; }

        switch (ModalMode)
        {
            case ModalMode.AddBundle:
                var bundle = new CosmeticBundle(CosmeticTypeDefinition.All) { Name = input };
                Bundles.Add(bundle);
                SelectedBundle = bundle;
                break;
            case ModalMode.RenameBundle when SelectedBundle != null:
                SelectedBundle.Name = input;
                break;
        }
        CancelModal();
    }

    [RelayCommand]
    private void CancelModal()
    {
        IsModalOpen = false;
        ModalMode = ModalMode.None;
        ModalInput = string.Empty;
    }

    // === Bundle/item actions ===

    public void DeleteBundle(CosmeticBundle bundle)
    {
        var index = Bundles.IndexOf(bundle);
        Bundles.Remove(bundle);
        if (SelectedBundle == bundle)
            SelectedBundle = Bundles.Count > 0 ? Bundles[Math.Max(0, Math.Min(index, Bundles.Count - 1))] : null;
    }

    public void AddCosmetic(CosmeticSection section)
    {
        if (SelectedBundle == null) return;
        var item = new CosmeticItem(section.TypeDefinition) { Name = "New " + section.TypeDefinition.Label };
        section.Items.Add(item);
        SelectedItem = item;
    }

    [RelayCommand]
    private void DeleteItem(CosmeticItem item)
    {
        if (SelectedBundle == null) return;
        var section = SelectedBundle.Sections.FirstOrDefault(s => s.TypeDefinition.Id == item.TypeDefinition.Id);
        section?.Items.Remove(item);
        if (SelectedItem == item) SelectedItem = null;
    }

    [RelayCommand]
    private void CompileBundle()
    {
        if (SelectedBundle == null) return;
        SelectedBundle.IsCompiled = true;
        SelectedBundle.CompiledDate = DateTime.Now;
    }

    partial void OnSelectedBundleChanged(CosmeticBundle? value)
    {
        SelectedItem = null;
    }
}
