using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(PlayerPhysics))]
internal static class PlayerPhysicsPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPhysics.FixedUpdate))]
    private static void FixedUpdatePostfix(PlayerPhysics __instance)
    {
        var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
        var player = __instance.myPlayer;
        if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim) return;

        if (player.cosmetics.hat.Hat != null)
        {
            var hatId = player.cosmetics.hat.Hat.name;
            var viewData = CosmeticsContext.Hats.TryGetViewData(hatId, out var value) ? value : null;
            SetHat(__instance, viewData);
        }

        if (player.AmOwner)
        {
            CosmeticsContext.UpdateAnimationFrames();
        }
    }
    
    private static void SetHat(PlayerPhysics physics, HatViewData? viewData)
    {
        var parent = physics.myPlayer.cosmetics.hat;
        if (parent == null || parent.Hat == null) return;
        if (!CosmeticsContext.Hats.TryGetCosmetic(parent.Hat.name, out var cosmetic))
        {
            return;
        }

        if (cosmetic.FrontAnimationFrames != null && cosmetic.FrontAnimationFrames.Count != 0)
        {
            parent.FrontLayer.sprite = cosmetic.FrontAnimationFrames[cosmetic.CurrentFrontFrame];
        }

        if (cosmetic.BackAnimationFrames != null && cosmetic.BackAnimationFrames.Count != 0)
        {
            parent.BackLayer.sprite = cosmetic.BackAnimationFrames[cosmetic.CurrentBackFrame];
        }

        if (cosmetic.FrontAnimationFrames?.Count == 0 && cosmetic.BackAnimationFrames?.Count == 0)
        {
            parent.FrontLayer.sprite = cosmetic.FlipResource != null && physics.FlipX ? cosmetic.FlipResource : viewData?.MainImage;
            parent.BackLayer.sprite = cosmetic.BackFlipResource != null && physics.FlipX ? cosmetic.BackFlipResource : viewData?.BackImage;
        }
    }
}