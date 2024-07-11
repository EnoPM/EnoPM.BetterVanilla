using Mono.Cecil;

namespace AmongUsDevKit.Extensions;

internal static class MemberDefinitionExtensions
{
    public static bool HasCustomAttribute(this IMemberDefinition member, string attributeName)
    {
        return member.CustomAttributes.Any(a => a.AttributeType.FullName == attributeName);
    }
    
    public static CustomAttribute? GetCustomAttribute(this IMemberDefinition member, string attributeName)
    {
        return member.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == attributeName);
    }
}