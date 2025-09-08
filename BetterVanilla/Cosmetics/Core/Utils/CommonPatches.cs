namespace BetterVanilla.Cosmetics.Core.Utils;

internal static class CommonPatches
{
    public static void InventoryTabOnEnable(this InventoryTab tab)
    {
        tab.PlayerPreview.gameObject.SetActive(true);
        if (tab.HasLocalPlayer())
        {
            tab.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        }
        else
        {
            tab.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
        }
    }
}