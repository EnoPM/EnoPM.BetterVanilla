using System;
using System.Collections.Generic;
using System.Reflection;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
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
        var localOptions = BetterVanillaManager.Instance.LocalOptions;
        pc.RpcSetTeamPreference(localOptions.TeamPreference.ParseValue(TeamPreferences.Both));
        if (!localOptions.ForcedTeamAssignment.IsLocked())
        {
            pc.RpcSetForcedTeamAssignment(localOptions.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
        }
        if (AmongUsClient.Instance.AmHost)
        {
            BetterVanillaManager.Instance.HostOptions.ShareAllOptions();
        }
    }

    public static void CustomSpawnHandshake(this PlayerControl pc)
    {
        if (!PlayerControl.LocalPlayer) return;
        var localOptions = BetterVanillaManager.Instance.LocalOptions;
        PlayerControl.LocalPlayer.RpcSetTeamPreference(localOptions.TeamPreference.ParseValue(TeamPreferences.Both));
        if (!pc.AmOwner && !localOptions.ForcedTeamAssignment.IsLocked())
        {
            PlayerControl.LocalPlayer.RpcSetForcedTeamAssignment(localOptions.ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
        }
    }

    private static MessageWriter StartRpcImmediately(this PlayerControl sender, RpcIds rpcId, PlayerControl receiver = null, bool reliable = true)
    {
        var targetId = receiver ? receiver.OwnerId : -1;
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