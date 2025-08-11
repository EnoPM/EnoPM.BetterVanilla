using System;
using System.Collections.Generic;
using System.Reflection;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using Hazel;

namespace BetterVanilla.Core.Extensions;

public static partial class PlayerControlRpcExtensions
{
    public const byte ReservedRpcCallId = 252;

    private delegate void RpcHandler(PlayerControl sender, MessageReader reader);

    private static readonly Dictionary<uint, RpcHandler> RpcHandlers = [];

    public static void HandleCustomRpc(this PlayerControl sender, MessageReader reader)
    {
        var rpcId = reader.ReadUInt32();
        if (!RpcHandlers.TryGetValue(rpcId, out var rpcHandler))
        {
            Ls.LogWarning($"No RPC handler found for id {rpcId} {((RpcIds)rpcId).ToString()} {RpcHandlers.Count}");
            return;
        }
        rpcHandler(sender, reader);
    }

    public static void CustomOwnerSpawnHandshake(this PlayerControl pc)
    {
        pc.RpcSendHandshake(BetterVanillaHandshake.Local);
        pc.RpcSetTeamPreference(LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both));
        if (FeatureOptions.Default.ForcedTeamAssignment.IsAllowed())
        {
            pc.RpcSetForcedTeamAssignment(FeatureOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
        }
        if (AmongUsClient.Instance.AmHost)
        {
            HostOptions.Default.ShareAllOptions();
        }
        var betterControl = pc.gameObject.GetComponent<BetterPlayerControl>();
        if (betterControl == null)
        {
            Ls.LogWarning($"Unable to find {nameof(BetterPlayerControl)} component for PlayerControl");
        }
        else if (betterControl.AmSponsor)
        {
            pc.RpcShareSponsorText(SponsorOptions.Default.SponsorText.Value);
            pc.RpcShareSponsorTextColor(SponsorOptions.Default.SponsorTextColor.Value);
            pc.RpcShareSponsorVisorColor(SponsorOptions.Default.VisorColor.Value);
        }
    }

    public static void CustomSpawnHandshake(this PlayerControl pc)
    {
        if (!PlayerControl.LocalPlayer) return;
        PlayerControl.LocalPlayer.RpcSendHandshake(BetterVanillaHandshake.Local);
        PlayerControl.LocalPlayer.RpcSetTeamPreference(LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both));
        if (!pc.AmOwner && FeatureOptions.Default.ForcedTeamAssignment.IsAllowed())
        {
            PlayerControl.LocalPlayer.RpcSetForcedTeamAssignment(FeatureOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
        }
        if (LocalConditions.AmSponsor())
        {
            Ls.LogMessage($"Sending sponsor rpcs...");
            SponsorOptions.Default.ShareSponsorText();
            SponsorOptions.Default.ShareSponsorTextColor();
            SponsorOptions.Default.ShareVisorColor();
        }
    }

    private static MessageWriter StartRpcImmediately(this PlayerControl sender, RpcIds rpcId, PlayerControl? receiver = null, bool reliable = true)
    {
        var targetId = receiver != null ? receiver.OwnerId : -1;
        var writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, ReservedRpcCallId, reliable ? SendOption.Reliable : SendOption.None, targetId);
        writer.Write((uint)rpcId);
        return writer;
    }

    [AttributeUsage(AttributeTargets.Method)]
    private sealed class RpcHandlerAttribute(RpcIds id) : Attribute
    {
        public readonly RpcIds RpcId = id;
    }

    static PlayerControlRpcExtensions()
    {
        var methods = typeof(PlayerControlRpcExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var rpcHandlerAttribute = method.GetCustomAttribute<RpcHandlerAttribute>();
            if (rpcHandlerAttribute == null) continue;
            if (!method.IsStatic)
            {
                Ls.LogError($"Unable to register rpc handler {method.Name}: method must be static");
                continue;
            }
            if (method.ContainsGenericParameters)
            {
                Ls.LogError($"Unable to register rpc handler {method.Name}: method have generic parameters");
                continue;
            }
            var parameters = method.GetParameters();
            if (parameters.Length != 2)
            {
                Ls.LogError($"Unable to register rpc handler {method.Name}: method must have 2 parameters");
                continue;
            }
            if (parameters[0].ParameterType != typeof(PlayerControl))
            {
                Ls.LogError($"Unable to register rpc handler {method.Name}: method parameter 1 must be of type {nameof(PlayerControl)}");
                continue;
            }
            if (parameters[1].ParameterType != typeof(MessageReader))
            {
                Ls.LogError($"Unable to register rpc handler {method.Name}: method parameter 2 must be of type {nameof(MessageReader)}");
                continue;
            }
            RpcHandlers[(uint)rpcHandlerAttribute.RpcId] = (RpcHandler)Delegate.CreateDelegate(typeof(RpcHandler), method);
        }
    }
}