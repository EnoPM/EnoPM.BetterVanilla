using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Animation;

namespace AmongUsCosmeticsManager.Views;

public partial class AnimationTimelinePanel : UserControl
{
    private ObservableCollection<AnimationNode>? _dragSourceCollection;
    private AnimationNode? _dragSourceNode;
    private Point _dragStartPoint;
    private bool _isDragging;

    public AnimationTimelinePanel()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, OnNodeDragOver);
        AddHandler(DragDrop.DropEvent, OnNodeDrop);
    }

    // === Node actions ===

    private async void OnAddFrameClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: FrameListValue frameList }) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = frameList.Definition.Label,
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Images") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } },
                FilePickerFileTypes.All
            }
        });

        foreach (var file in files)
            frameList.Nodes.Add(new FrameNode { Data = File.ReadAllBytes(file.Path.LocalPath) });
    }

    private void OnAddDelayClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: FrameListValue frameList }) return;
        frameList.Nodes.Add(new DelayNode());
    }

    private void OnAddLoopClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: FrameListValue frameList }) return;
        frameList.Nodes.Add(new LoopNode());
    }

    private void OnRemoveNodeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control control) return;
        if (control.DataContext is not AnimationNode node) return;
        FindOwnerCollection(control)?.Remove(node);
    }

    // === Drag-and-drop reorder ===

    private void OnNodePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control) return;
        if (!e.GetCurrentPoint(control).Properties.IsLeftButtonPressed) return;
        if (control.DataContext is not AnimationNode node) return;

        _dragStartPoint = e.GetPosition(control);
        _isDragging = false;
        _dragSourceNode = node;
        _dragSourceCollection = FindOwnerCollection(control);
    }

    private async void OnNodePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragSourceNode == null || _isDragging) return;
        if (sender is not Control control) return;
        if (!e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
        {
            ResetDragState();
            return;
        }

        var pos = e.GetPosition(control);
        var diff = pos - _dragStartPoint;
        if (Math.Abs(diff.X) < 5 && Math.Abs(diff.Y) < 5) return;

        _isDragging = true;

#pragma warning disable CS0618
        var data = new DataObject();
        data.Set("NodeDrag", "active");
        await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
#pragma warning restore CS0618

        _isDragging = false;
        ResetDragState();
    }

    private void OnNodeDragOver(object? sender, DragEventArgs e)
    {
        if (_dragSourceNode == null) { e.DragEffects = DragDropEffects.None; return; }

        var target = FindDropTarget(e.Source as Control);
        e.DragEffects = target?.DataContext is AnimationNode ? DragDropEffects.Move : DragDropEffects.None;
    }

    private void OnNodeDrop(object? sender, DragEventArgs e)
    {
        if (_dragSourceNode == null || _dragSourceCollection == null) return;

        var target = FindDropTarget(e.Source as Control);
        if (target?.DataContext is not AnimationNode targetNode) return;
        if (ReferenceEquals(_dragSourceNode, targetNode)) return;

        var targetCollection = FindOwnerCollection(target);
        if (targetCollection == null) return;

        var sourceIndex = _dragSourceCollection.IndexOf(_dragSourceNode);
        var targetIndex = targetCollection.IndexOf(targetNode);
        if (sourceIndex < 0 || targetIndex < 0) return;

        if (ReferenceEquals(_dragSourceCollection, targetCollection))
        {
            _dragSourceCollection.Move(sourceIndex, targetIndex);
        }
        else
        {
            _dragSourceCollection.RemoveAt(sourceIndex);
            targetCollection.Insert(targetIndex, _dragSourceNode);
        }

        ResetDragState();
    }

    private void ResetDragState()
    {
        _dragSourceNode = null;
        _dragSourceCollection = null;
    }

    private static Control? FindDropTarget(Control? control)
    {
        var current = control;
        while (current != null)
        {
            if (current.DataContext is AnimationNode && DragDrop.GetAllowDrop(current))
                return current;
            current = current.Parent as Control;
        }
        return null;
    }

    private static ObservableCollection<AnimationNode>? FindOwnerCollection(Control control)
    {
        var current = control.Parent;
        while (current != null)
        {
            if (current is ItemsControl ic && ic.ItemsSource is ObservableCollection<AnimationNode> list)
                return list;
            current = current.Parent;
        }
        return null;
    }
}
