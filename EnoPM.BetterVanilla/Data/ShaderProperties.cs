using UnityEngine;

namespace EnoPM.BetterVanilla.Data;

public static class ShaderProperties
{
    // Float
    public static readonly int Percent = Shader.PropertyToID("_Percent");
    
    // Texture
    public static readonly int MainTexture = Shader.PropertyToID("_MainTex");
    
    // Color
    public static readonly int Color = Shader.PropertyToID("_Color");
    
    // Float
    public static readonly int Desat = Shader.PropertyToID("_Desat");
    
    // Vector
    public static readonly int NormalizedUvs = Shader.PropertyToID("_NormalizedUvs");
    
    // Color
    public static readonly int UnderlayColor = Shader.PropertyToID("_UnderlayColor");
    
    // Float
    public static readonly int Outline = Shader.PropertyToID("_Outline");
    
    // Color
    public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    
    // Color
    public static readonly int AddColor = Shader.PropertyToID("_AddColor");
}