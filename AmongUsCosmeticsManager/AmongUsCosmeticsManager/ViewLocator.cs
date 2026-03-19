using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AmongUsCosmeticsManager.ViewModels;
using AmongUsCosmeticsManager.Views;

namespace AmongUsCosmeticsManager;

public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> ViewMap = new()
    {
        [typeof(MainViewModel)] = () => new MainView()
    };

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        if (ViewMap.TryGetValue(param.GetType(), out var factory))
            return factory();

        return new TextBlock { Text = "Not Found: " + param.GetType().Name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
