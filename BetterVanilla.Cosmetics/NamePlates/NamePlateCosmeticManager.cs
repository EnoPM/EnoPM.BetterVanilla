using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Cosmetics.Core.Manager;

namespace BetterVanilla.Cosmetics.NamePlates;

public class NamePlateCosmeticManager : BaseCosmeticManager<NamePlateCosmetic, NamePlateViewData, PlayerVoteArea, NamePlateData>
{
    protected override bool CanBeCached(PlayerVoteArea parent)
    {
        return parent != null && parent.Background != null;
    }

    protected override void PopulateParent(PlayerVoteArea parent)
    {
        
    }

    protected override List<NamePlateData> GetVanillaCosmeticData()
    {
        return HatManager.Instance.allNamePlates.ToList();
    }

    protected override void OverrideVanillaCosmeticData(List<NamePlateData> allCosmeticData)
    {
        HatManager.Instance.allNamePlates = allCosmeticData.ToArray();
    }

    protected override PlayerVoteArea? GetPlayerParent(PlayerControl player)
    {
        if (player == null || player.Data == null) return null;
        if (!MeetingHud.Instance) return null;
        return MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);
    }

    public override void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        
    }

    public override void UpdateMaterialFromViewAsset(PlayerVoteArea parent, NamePlateViewData asset)
    {
        
    }

    public override void PopulateParentFromAsset(PlayerVoteArea parent, NamePlateViewData asset)
    {
        parent.Background.sprite = asset.Image;
    }
}