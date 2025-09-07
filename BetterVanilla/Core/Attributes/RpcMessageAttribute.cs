using System;
using BetterVanilla.Core.Data;

namespace BetterVanilla.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RpcMessageAttribute(RpcIds id) : Attribute
{
    public RpcIds Id { get; } = id;
}