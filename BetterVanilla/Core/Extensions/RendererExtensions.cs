using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class RendererExtensions
{
    public static void SetVisorColor(this Renderer renderer, Color color)
    {
        if (renderer.material.GetColor(PlayerMaterial.VisorColor) == color) return;
        renderer.material.SetColor(PlayerMaterial.VisorColor, color);
    }
}