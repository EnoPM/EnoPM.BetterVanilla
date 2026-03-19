using System.Collections.ObjectModel;
using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class CosmeticSection : ObservableObject
{
    public CosmeticTypeDefinition TypeDefinition { get; }
    public ObservableCollection<CosmeticItem> Items { get; } = [];

    public CosmeticSection(CosmeticTypeDefinition typeDefinition)
    {
        TypeDefinition = typeDefinition;
    }
}
