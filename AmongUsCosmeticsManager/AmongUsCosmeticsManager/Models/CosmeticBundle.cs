using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class CosmeticBundle : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isCompiled;

    [ObservableProperty]
    private DateTime? _compiledDate;

    public ObservableCollection<CosmeticSection> Sections { get; } = [];

    public CosmeticBundle(IEnumerable<CosmeticTypeDefinition> typeDefinitions)
    {
        foreach (var typeDef in typeDefinitions)
            Sections.Add(new CosmeticSection(typeDef));
    }

    public CosmeticSection? GetSection(string typeId)
        => Sections.FirstOrDefault(s => s.TypeDefinition.Id == typeId);

    public int TotalItems => Sections.Sum(s => s.Items.Count);
}
