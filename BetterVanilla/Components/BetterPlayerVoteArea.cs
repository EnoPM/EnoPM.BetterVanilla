using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerVoteArea : MonoBehaviour
{
    public static readonly List<BetterPlayerVoteArea> AllVoteAreas = [];
    
    public PlayerVoteArea VoteArea { get; private set; }
    private TextMeshPro InfosText { get; set; }

    private void Awake()
    {
        VoteArea = GetComponent<PlayerVoteArea>();
        AllVoteAreas.Add(this);
    }

    private void Start()
    {
        this.StartCoroutine(CoStart());
    }

    private void OnDestroy()
    {
        AllVoteAreas.Remove(this);
    }

    private IEnumerator CoStart()
    {
        while (!VoteArea || !VoteArea.NameText)
        {
            yield return new WaitForEndOfFrame();
        }

        InfosText = Instantiate(VoteArea.NameText, VoteArea.NameText.transform.parent);
        var pos = InfosText.transform.localPosition;
        pos.y = 0.2f;
        InfosText.transform.localPosition = pos;
        InfosText.SetText(string.Empty);
    }

    private void Update()
    {
        if (!InfosText) return;
        if (MeetingHud.Instance.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
        {
            InfosText.gameObject.SetActive(false);
            return;
            
        }
        var player = BetterVanillaManager.Instance.AllPlayers.Find(x => x.Player.PlayerId == VoteArea.TargetPlayerId);
        if (!player)
        {
            InfosText.gameObject.SetActive(false);
            return;
        }
        InfosText.SetText(player.GetBetterInfosText());
        InfosText.gameObject.SetActive(true);
    }
}