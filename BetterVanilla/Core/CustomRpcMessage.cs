using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core;

public abstract class CustomRpcMessage
{
    internal const byte ReservedRpcCallId = 252;
    private delegate CustomRpcMessage RpcHandlerDelegate(BetterPlayerControl sender, MessageReader reader);

    private static Dictionary<RpcIds, RpcHandlerDelegate> MessageHandlers { get; } = new();

    static CustomRpcMessage()
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(CustomRpcMessage)) && !x.IsAbstract);
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RpcMessageAttribute>(false);
            if (attribute == null)
            {
                Ls.LogError($"{nameof(RpcMessageAttribute)} is required in rpc message type: {type.FullName}");
                continue;
            }

            var constructor = type.GetConstructor([typeof(BetterPlayerControl), typeof(MessageReader)]);
            if (constructor == null)
            {
                Ls.LogError($"{type.FullName} require a ({nameof(BetterPlayerControl)}, {nameof(MessageReader)}) constructor");
                continue;
            }

            var senderParameter = Expression.Parameter(typeof(BetterPlayerControl), "sender");
            var readerParameter = Expression.Parameter(typeof(MessageReader), "reader");
            var expression = Expression.New(constructor, senderParameter, readerParameter);
            var lambda = Expression.Lambda<RpcHandlerDelegate>(
                expression,
                senderParameter,
                readerParameter
            );

            MessageHandlers[attribute.Id] = lambda.Compile();
        }
    }

    internal static void HandleRpcMessage(PlayerControl sender, MessageReader reader)
    {
        var rpcId = (RpcIds)reader.ReadUInt32();
        if (!MessageHandlers.TryGetValue(rpcId, out var handler))
        {
            Ls.LogError($"Rpc message handler {rpcId.ToString()} not found among {MessageHandlers.Count} handlers");
            return;
        }
        var player = sender.gameObject.GetComponent<BetterPlayerControl>();
        handler(player, reader).HandleMessage();
    }
    
    protected BetterPlayerControl Sender { get; }

    protected CustomRpcMessage(BetterPlayerControl sender)
    {
        Sender = sender;
    }

    protected abstract void WriteMessage(MessageWriter writer);
    protected abstract void HandleMessage();

    public virtual void Send(BetterPlayerControl? receiver = null)
    {
        var attribute = GetType().GetCustomAttribute<RpcMessageAttribute>(false);
        if (attribute == null)
        {
            throw new Exception($"{nameof(RpcMessageAttribute)} is required in rpc message type: {GetType().FullName}");
        }

        var targetId = receiver?.Player.OwnerId ?? -1;
        var writer = AmongUsClient.Instance.StartRpcImmediately(Sender.Player.NetId, ReservedRpcCallId, SendOption.Reliable, targetId);
        writer.Write((uint)attribute.Id);
        WriteMessage(writer);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}