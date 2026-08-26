using System.Collections;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerVoteArea : MonoBehaviour
{
    private PlayerVoteArea? VoteArea { get; set; }
    private BetterPlayerTexts? InfosText { get; set; }

    private void Awake()
    {
        VoteArea = GetComponent<PlayerVoteArea>();
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        while (VoteArea == null)
        {
            yield return new WaitForEndOfFrame();
        }
        InfosText = Instantiate(BetterVanillaManager.Instance.BetterVoteAreaTextsPrefab, VoteArea.NameText.transform.parent);
        InfosText.gameObject.hideFlags |= HideFlags.HideAndDontSave;
        InfosText.gameObject.SetActive(true);

        while (!InfosText.IsReady)
        {
            yield return new WaitForEndOfFrame();
        }
        InfosText.SetSponsorText(string.Empty);
        InfosText.SetMainText(string.Empty);
    }

    private void Update()
    {
        if (VoteArea == null || InfosText == null || !MeetingHud.Instance || !InfosText.IsReady) return;
        if (MeetingHud.Instance.CurrentState is MeetingHud.MeetingStates.Animating or MeetingHud.MeetingStates.Proceeding or MeetingHud.MeetingStates.Results)
        {
            InfosText.gameObject.SetActive(false);
            return;
            
        }
        var player = BetterVanillaManager.Instance.GetPlayerById(VoteArea.PlayerId.Value);
        if (player == null)
        {
            InfosText.gameObject.SetActive(false);
            return;
        }
        VoteArea.PlayerIcon.SetVisorColor(player.GetVisorColor());
        InfosText.SetSponsorText(player.GetSponsorText());
        InfosText.SetMainText(player.GetBetterInfosText());
        InfosText.gameObject.SetActive(true);
        InfosText.SetSponsorTextActive(!DataManager.Settings.Accessibility.ColorBlindMode);
    }
}
