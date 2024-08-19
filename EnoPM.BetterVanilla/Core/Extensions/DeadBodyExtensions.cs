using EnoPM.BetterVanilla.Core.Data;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core.Extensions;

public static class DeadBodyExtensions
{
    public static void SetOutline(this DeadBody db, Color color, float size)
    {
        if (!db || db.bodyRenderers[0] == null) return;
        db.bodyRenderers[0].material.SetFloat(ShaderProperties.Outline, size);
        db.bodyRenderers[0].material.SetColor(ShaderProperties.OutlineColor, color);
    }
}