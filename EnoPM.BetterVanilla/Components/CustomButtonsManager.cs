using System;
using System.Collections.Generic;
using System.Reflection;
using EnoPM.BetterVanilla.Buttons;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Attributes;
using EnoPM.BetterVanilla.Core.Extensions;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class CustomButtonsManager : MonoBehaviour
{
    private static readonly List<Type> ButtonTypes = [];

    private void Start()
    {
        AddButton<ModMenuHudButton>(ModSettings.Local.ModMenuButtonPosition);
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
        ButtonTypes.Add(type);
    }
    
    public static CustomButtonsManager Instance { get; internal set; }
    
    private GameObject BaseContainer { get; set; }

    private List<CustomGameplayButton> Buttons { get; } = [];
    public Dictionary<ButtonPositions, GameObject> Positions { get; set; }

    private void Awake()
    {
        if (Instance)
        {
            throw new Exception($"{nameof(CustomButtonsManager)} must be a singleton");
        }
        Instance = this;
        BaseContainer = HudManager.Instance.UseButton.transform.parent.gameObject;
        
        Positions = new Dictionary<ButtonPositions, GameObject>
        {
            { ButtonPositions.BottomRight, BaseContainer }
        };
        
        RegisterDerived(ButtonPositions.BottomLeft, AspectPosition.EdgeAlignments.LeftBottom);
        RegisterDerived(ButtonPositions.MiddleLeft, AspectPosition.EdgeAlignments.Left);
        RegisterDerived(ButtonPositions.TopLeft, AspectPosition.EdgeAlignments.LeftTop);
    }

    private void RegisterDerived(ButtonPositions position, AspectPosition.EdgeAlignments edgeAlignment)
    {
        if (Positions.ContainsKey(position)) return;
        Positions.Add(position, CreateDerived(edgeAlignment));
    }

    private GameObject CreateDerived(AspectPosition.EdgeAlignments alignment)
    {
        var derived = Instantiate(BaseContainer, BaseContainer.transform.parent);
        derived.name = alignment.ToString();
        derived.DestroyChildren();
        var aspectPosition = derived.GetComponent<AspectPosition>();
        aspectPosition.Alignment = alignment;
        aspectPosition.AdjustPosition();

        return derived;
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
    
    public T AddButton<T>(ButtonPositions buttonPosition) where T : CustomGameplayButton
    {
        var type = typeof(T);
        var button = Positions[buttonPosition].AddComponent<T>();
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
    
    public enum ButtonPositions
    {
        [DisplayAs("Bottom Right")]
        BottomRight,
    
        [DisplayAs("Bottom Left")]
        BottomLeft,
    
        [DisplayAs("Middle Left")]
        MiddleLeft,
    
        [DisplayAs("Top Left")]
        TopLeft
    }
}