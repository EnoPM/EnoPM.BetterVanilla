using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class CategoryHeaderMaskedExtensions
{
    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
    private static readonly int Stencil = Shader.PropertyToID("_Stencil");
    
    public static void CustomSetHeader(this CategoryHeaderMasked headerMasked, int maskLayer)
    {
        headerMasked.Title.SetText("Better Vanilla");
        headerMasked.Background.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
        if (headerMasked.Divider)
        {
            headerMasked.Divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
        }
        headerMasked.Title.fontMaterial.SetFloat(StencilComp, 3f);
        headerMasked.Title.fontMaterial.SetFloat(Stencil, maskLayer);
    }
}