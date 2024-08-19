using System;
using System.Collections.Generic;
using System.Reflection;
using EnoPM.BetterVanilla.Buttons;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Attributes;
using EnoPM.BetterVanilla.Core.Data;
using EnoPM.BetterVanilla.Core.Extensions;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class CustomButtonsManager : MonoBehaviour
{
    private static readonly List<RegisteredCustomButton> ButtonTypes = [];

    private void Start()
    {
        Plugin.Logger.LogMessage($"ButtonTypes: {ButtonTypes.Count}");
        AddButton<ModMenuHudButton>();
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        try
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                RegisterType(type);
            }
        }
        catch (ReflectionTypeLoadException exception)
        {
            foreach (var type in exception.Types)
            {
                RegisterType(type);
            }
        }
    }

    private static void RegisterType(Type type)
    {
        if (type == typeof(CustomGameplayButton) || !type.IsAssignableTo(typeof(CustomGameplayButton))) return;
        var attribute = type.GetCustomAttribute<ButtonConfigAttribute>();
        if (attribute == null)
        {
            Plugin.Logger.LogWarning($"Unable to load CustomButton type {type.FullName}. Attribute ButtonConfig is missing");
            return;
        }
        ButtonTypes.Add(new RegisteredCustomButton(type, attribute));
    }
    
    public static CustomButtonsManager Instance { get; internal set; }
    
    private GameObject RightBottomContainer { get; set; }
    private GameObject LeftBottomContainer { get; set; }

    private List<CustomGameplayButton> Buttons { get; } = [];
    private Dictionary<ButtonPositions, GameObject> Positions { get; set; }

    private void Awake()
    {
        if (Instance)
        {
            throw new Exception($"{nameof(CustomButtonsManager)} must be a singleton");
        }
        Instance = this;
        RightBottomContainer = HudManager.Instance.UseButton.transform.parent.gameObject;
        LeftBottomContainer = Instantiate(RightBottomContainer, RightBottomContainer.transform.parent);
        LeftBottomContainer.name = "LeftBottom";
        LeftBottomContainer.DestroyChildren();
        var aspectPosition = LeftBottomContainer.GetComponent<AspectPosition>();
        aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
        aspectPosition.AdjustPosition();
        
        Positions = new Dictionary<ButtonPositions, GameObject>
        {
            { ButtonPositions.BottomLeft, LeftBottomContainer },
            { ButtonPositions.BottomRight, RightBottomContainer }
        };
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void RemoveAllButtons()
    {
        foreach (var button in Buttons)
        {
            Destroy(button);
        }
    }
    
    public T AddButton<T>() where T : CustomGameplayButton
    {
        var type = typeof(T);
        var data = ButtonTypes.Find(x => x.Type == type);
        var button = Positions[data.Config.Position].AddComponent<T>();
        button.SetActive(true);
        Buttons.Add(button);
        return button;
    }

    public T GetButton<T>() where T : CustomGameplayButton
    {
        var type = typeof(T);
        var button = Buttons.Find(x => x.GetType() == type);
        if (!button) return null;
        return button as T;
    }
    
    private class RegisteredCustomButton(Type type, ButtonConfigAttribute config)
    {
        public readonly Type Type = type;
        public readonly ButtonConfigAttribute Config = config;
    }
}