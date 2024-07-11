using System;

namespace AmongUsDevKit.Api;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AssetAttribute(string path) : Attribute;