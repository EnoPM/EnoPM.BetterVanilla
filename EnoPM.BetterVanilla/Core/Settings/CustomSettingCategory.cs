using System;
using System.Collections.Generic;

namespace EnoPM.BetterVanilla.Core.Settings;

public sealed class CustomSettingCategory : BaseSettingsCreator
{
    public static readonly List<CustomSettingCategory> AllCategories = [];
    public static CustomSettingCategory GetCategory(string id) => AllCategories.Find(x => x.Id == id);
    
    public readonly string Id;
    
    public CustomSettingCategory(string id, Func<bool> isEditableFunc = null) : base(isEditableFunc)
    {
        Id = id;
        AllCategories.Add(this);
    }
}