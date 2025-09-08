using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(PlayerPhysics))]
internal static class PlayerPhysicsPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPhysics.FixedUpdate))]
    private static void FixedUpdatePostfix(PlayerPhysics __instance)
    {
        var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
        var player = __instance.myPlayer;
        if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim) return;

        CosmeticsManager.RefreshAnimationFrames(__instance);

        if (player.AmOwner)
        {
            CosmeticsManager.UpdateAnimationFrames();
        }
    }
}