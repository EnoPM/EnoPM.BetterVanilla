using Mono.Cecil;

namespace AmongUsDevKit.Extensions;

internal static class TypeReferenceExtensions
{
    public static GenericInstanceType CreateGenericType(this TypeReference type, params TypeReference[] typeArguments)
    {
        var a = new GenericInstanceType(type.Resolve());
        var genericType = new GenericInstanceType(type);
        foreach (var typeArgument in typeArguments)
        {
            genericType.GenericArguments.Add(typeArgument);
        }

        return genericType;
    }
}