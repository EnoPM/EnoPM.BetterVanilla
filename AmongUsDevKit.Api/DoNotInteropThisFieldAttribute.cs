using System;

namespace AmongUsDevKit.Api;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DoNotInteropThisFieldAttribute : Attribute;