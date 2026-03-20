using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AmongUsCosmeticsManager.Models.Animation;
using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class FrameListValue : ObservableObject
{
    public FrameListDefinition Definition { get; }
    public ObservableCollection<AnimationNode> Nodes { get; } = [];

    [ObservableProperty]
    private int _defaultFps = 10;

    [ObservableProperty]
    private AnimationNode? _playheadNode;

    public bool IsPlaybackActive => PlayheadNode != null;

    partial void OnPlayheadNodeChanged(AnimationNode? oldValue, AnimationNode? newValue)
    {
        if (oldValue != null) oldValue.IsPlayheadHere = false;
        if (newValue != null) newValue.IsPlayheadHere = true;
        OnPropertyChanged(nameof(IsPlaybackActive));
    }

    partial void OnDefaultFpsChanged(int value)
    {
        UpdateEffectiveDurations();
    }

    public FrameListValue(FrameListDefinition definition)
    {
        Definition = definition;
        Nodes.CollectionChanged += OnNodesChanged;
    }

    private void OnNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var node in e.NewItems.OfType<FrameNode>())
            {
                node.SetEffectiveFromFps(DefaultDurationMs);
            }
        }
    }

    private int DefaultDurationMs => 1000 / Math.Max(1, DefaultFps);

    private void UpdateEffectiveDurations()
    {
        var defaultMs = DefaultDurationMs;
        foreach (var frame in Nodes.OfType<FrameNode>())
        {
            frame.SetEffectiveFromFps(defaultMs);
        }
    }

    public byte[]? FirstFrameData => Nodes.OfType<FrameNode>().FirstOrDefault()?.Data;

    public void MoveNode(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= Nodes.Count) return;
        if (toIndex < 0 || toIndex >= Nodes.Count) return;
        if (fromIndex == toIndex) return;
        Nodes.Move(fromIndex, toIndex);
    }

    public void RemoveNodeAt(int index)
    {
        if (index >= 0 && index < Nodes.Count)
            Nodes.RemoveAt(index);
    }
}
