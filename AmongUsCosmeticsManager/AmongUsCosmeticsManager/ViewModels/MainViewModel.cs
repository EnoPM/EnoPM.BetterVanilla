using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
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
    private ResourceGroup? _editingResourceGroup;

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

    [ObservableProperty]
    private byte[]? _recoloredPlayerTemplateFlip;

    [ObservableProperty]
    private byte[]? _recoloredPlayerTemplateClimb;

    // Animation playback
    private DispatcherTimer? _animationTimer;
    private Dictionary<string, List<PlaybackStep>>? _playbackPlans;
    private int _currentStepIndex;
    private bool _needsPlanRebuild;

    [ObservableProperty]
    private bool _isAnimationPlaying;

    [ObservableProperty]
    private byte[]? _currentFrontFrameData;

    [ObservableProperty]
    private byte[]? _currentBackFrameData;

    public AppConfig AppConfig { get; }
    public ObservableCollection<CosmeticBundle> Bundles { get; } = [];
    public bool HasProjectFile => _projectService.HasProject;
    public PlayerColor[] PlayerColors => PlayerPalette.Colors;

    public MainViewModel() : this(new ConfigService(), new ProjectService()) { }

    private byte[]? _playerTemplateRaw;
    private byte[]? _playerTemplateClimbRaw;

    private static readonly SkiaSharp.SKColor VisorColor = new(149, 202, 220);

    public MainViewModel(ConfigService configService, ProjectService projectService)
    {
        _projectService = projectService;
        configService.Load();
        AppConfig = configService.AppConfig;
        LoadPlayerTemplates();
        UpdateRecoloredTemplates();
    }

    private void LoadPlayerTemplates()
    {
        _playerTemplateRaw = LoadEmbeddedResource("AmongUsCosmeticsManager.Assets.player-template.png");
        _playerTemplateClimbRaw = LoadEmbeddedResource("AmongUsCosmeticsManager.Assets.player-template-climb.png");
    }

    private static byte[]? LoadEmbeddedResource(string name)
    {
        var assembly = typeof(MainViewModel).Assembly;
        using var stream = assembly.GetManifestResourceStream(name);
        if (stream == null) return null;
        using var ms = new System.IO.MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private void UpdateRecoloredTemplates()
    {
        var body = SelectedPlayerColor.Body;
        var shadow = SelectedPlayerColor.Shadow;

        if (_playerTemplateRaw != null)
        {
            RecoloredPlayerTemplate = RecolorService.Recolor(_playerTemplateRaw, body, VisorColor, shadow);
            RecoloredPlayerTemplateFlip = RecolorService.Recolor(_playerTemplateRaw, body, VisorColor, shadow);
        }

        if (_playerTemplateClimbRaw != null)
            RecoloredPlayerTemplateClimb = RecolorService.Recolor(_playerTemplateClimbRaw, body, VisorColor, shadow);
    }

    partial void OnSelectedPlayerColorChanged(PlayerColor value)
    {
        UpdateRecoloredTemplates();
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

    [ObservableProperty]
    private string? _lastError;

    [ObservableProperty]
    private bool _isImporting;

    [ObservableProperty]
    private string _importStatus = string.Empty;

    public async void ImportLegacyBundle(string filePath)
    {
        await ImportAsync(progress => BundleImporter.Import(filePath, progress));
    }

    public async void ImportCosmeticsBundle(string filePath)
    {
        await ImportAsync(progress => CosmeticsBundleImporter.Import(filePath, progress));
    }

    private async System.Threading.Tasks.Task ImportAsync(Func<IProgress<string>, CosmeticBundle> importFunc)
    {
        if (IsImporting) return;

        IsImporting = true;
        ImportStatus = "Démarrage...";
        LastError = null;

        var progress = new Progress<string>(status =>
        {
            ImportStatus = status;
        });

        try
        {
            var bundle = await System.Threading.Tasks.Task.Run(() => importFunc(progress));
            Bundles.Add(bundle);
            SelectedBundle = bundle;
        }
        catch (Exception ex)
        {
            LastError = $"Import failed: {ex.Message}";
        }
        finally
        {
            IsImporting = false;
            ImportStatus = string.Empty;
        }
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

    [ObservableProperty]
    private string? _lastCompilePath;

    [ObservableProperty]
    private bool _isCompiling;

    [ObservableProperty]
    private string _compileStatus = string.Empty;

    public async void ExportCosmeticsBundle(string outputPath)
    {
        await ExportAsync(outputPath, "Cosmetics Bundle", bundle => BundleCompileService.Compile(bundle));
    }

    public async void ExportLegacyBundle(string outputPath)
    {
        await ExportAsync(outputPath, "Bundle Legacy", bundle => LegacyBundleExporter.Export(bundle));
    }

    private async System.Threading.Tasks.Task ExportAsync(string outputPath, string formatName, Func<CosmeticBundle, byte[]> exportFunc)
    {
        if (SelectedBundle == null || IsCompiling) return;

        IsCompiling = true;
        CompileStatus = "Démarrage...";
        LastError = null;

        try
        {
            var bundle = SelectedBundle;
            CompileStatus = $"Compilation du bundle \"{bundle.Name}\" ({formatName})...";

            var data = await System.Threading.Tasks.Task.Run(() => exportFunc(bundle));

            CompileStatus = "Écriture du fichier...";
            await System.Threading.Tasks.Task.Run(() => System.IO.File.WriteAllBytes(outputPath, data));

            bundle.IsCompiled = true;
            bundle.CompiledDate = DateTime.Now;
            LastCompilePath = outputPath;
        }
        catch (Exception ex)
        {
            LastError = $"Export failed: {ex.Message}";
        }
        finally
        {
            IsCompiling = false;
            CompileStatus = string.Empty;
        }
    }

    partial void OnSelectedBundleChanged(CosmeticBundle? value)
    {
        SelectedItem = null;
    }

    partial void OnSelectedItemChanged(CosmeticItem? value)
    {
        StopAnimation();
        EditingResourceGroup = null;
    }

    [RelayCommand]
    private void EditResourceGroup(ResourceGroup group) => EditingResourceGroup = group;

    [RelayCommand]
    private void CloseAnimationEditor() => EditingResourceGroup = null;

    // === Animation ===

    [RelayCommand]
    private void ToggleAnimation()
    {
        if (IsAnimationPlaying) StopAnimation();
        else StartAnimation();
    }

    private void StartAnimation()
    {
        if (SelectedItem == null) return;
        if (!SelectedItem.FrameLists.Any(fl => fl.Nodes.Count > 0)) return;

        RebuildPlaybackPlans();
        if (_playbackPlans == null || _playbackPlans.Count == 0) return;

        _currentStepIndex = 0;
        IsAnimationPlaying = true;

        // Subscribe to node changes for live rebuild
        foreach (var fl in SelectedItem.FrameLists)
            SubscribeToAllNodes(fl);

        ApplyCurrentStep();

        _animationTimer = new DispatcherTimer();
        _animationTimer.Tick += OnAnimationTick;
        ScheduleNextTick();
        _animationTimer.Start();
    }

    public void StopAnimation()
    {
        _animationTimer?.Stop();
        _animationTimer = null;
        _playbackPlans = null;
        IsAnimationPlaying = false;
        CurrentFrontFrameData = null;
        CurrentBackFrameData = null;

        if (SelectedItem != null)
        {
            foreach (var fl in SelectedItem.FrameLists)
            {
                fl.PlayheadNode = null;
                UnsubscribeFromAllNodes(fl);
            }
        }
    }

    private void OnNodesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (Models.Animation.AnimationNode node in e.OldItems)
                node.PropertyChanged -= OnNodePropertyChanged;

        if (e.NewItems != null)
            foreach (Models.Animation.AnimationNode node in e.NewItems)
                node.PropertyChanged += OnNodePropertyChanged;

        _needsPlanRebuild = true;
    }

    private void OnNodePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _needsPlanRebuild = true;
    }

    private void SubscribeToAllNodes(Models.FrameListValue fl)
    {
        fl.PropertyChanged += OnNodePropertyChanged;
        fl.Nodes.CollectionChanged += OnNodesCollectionChanged;
        foreach (var node in fl.Nodes)
            node.PropertyChanged += OnNodePropertyChanged;
    }

    private void UnsubscribeFromAllNodes(Models.FrameListValue fl)
    {
        fl.PropertyChanged -= OnNodePropertyChanged;
        fl.Nodes.CollectionChanged -= OnNodesCollectionChanged;
        foreach (var node in fl.Nodes)
            node.PropertyChanged -= OnNodePropertyChanged;
    }

    private void RebuildPlaybackPlans()
    {
        if (SelectedItem == null) return;

        _playbackPlans = new Dictionary<string, List<PlaybackStep>>();
        foreach (var fl in SelectedItem.FrameLists)
        {
            var plan = AnimationPlaybackEngine.BuildPlan(fl.Nodes, fl.DefaultFps);
            if (plan.Count > 0)
                _playbackPlans[fl.Definition.Id] = plan;
        }
        _needsPlanRebuild = false;
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        if (_needsPlanRebuild)
        {
            RebuildPlaybackPlans();
            if (_playbackPlans == null || _playbackPlans.Count == 0) { StopAnimation(); return; }
            _currentStepIndex = Math.Min(_currentStepIndex, _playbackPlans.Values.Max(p => p.Count) - 1);
        }

        _currentStepIndex++;

        var maxLen = _playbackPlans?.Values.Max(p => p.Count) ?? 0;
        if (maxLen == 0) { StopAnimation(); return; }

        if (_currentStepIndex >= maxLen)
            _currentStepIndex = 0;

        ApplyCurrentStep();
        ScheduleNextTick();
    }

    private void ApplyCurrentStep()
    {
        if (_playbackPlans == null || SelectedItem == null) return;

        CurrentFrontFrameData = GetStepFrame("frontAnimation");
        CurrentBackFrameData = GetStepFrame("backAnimation");

        foreach (var fl in SelectedItem.FrameLists)
        {
            if (_playbackPlans.TryGetValue(fl.Definition.Id, out var plan) && plan.Count > 0)
            {
                var step = plan[_currentStepIndex % plan.Count];
                fl.PlayheadNode = step.SourceNode;
            }
            else
            {
                fl.PlayheadNode = null;
            }
        }
    }

    private byte[]? GetStepFrame(string frameListId)
    {
        if (_playbackPlans == null || !_playbackPlans.TryGetValue(frameListId, out var plan) || plan.Count == 0)
            return null;

        var step = plan[_currentStepIndex % plan.Count];
        // For delay steps (null frame), keep showing the last frame
        if (step.FrameData != null) return step.FrameData;

        // Walk backward to find the last non-null frame
        for (var i = _currentStepIndex % plan.Count - 1; i >= 0; i--)
        {
            if (plan[i].FrameData != null) return plan[i].FrameData;
        }
        return null;
    }

    private void ScheduleNextTick()
    {
        if (_animationTimer == null || _playbackPlans == null) return;

        // Use the shortest step duration across all active plans
        var duration = int.MaxValue;
        foreach (var plan in _playbackPlans.Values)
        {
            if (plan.Count == 0) continue;
            var step = plan[_currentStepIndex % plan.Count];
            duration = Math.Min(duration, step.DurationMs);
        }

        _animationTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(1, duration == int.MaxValue ? 100 : duration));
    }

}
