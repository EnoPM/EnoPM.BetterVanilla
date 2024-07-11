using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AmongUsDevKit.Il2Cpp;

public sealed class AmongUsReferenceHelper(AssemblyDefinition assembly, AmongUsPluginResolver resolver)
{
    private readonly Dictionary<string, TypeDefinition?> _localTypeCache = [];
    private readonly Dictionary<string, TypeDefinition?> _referenceTypeCache = [];

    public TypeDefinition? FindLocalTypeDefinition(string fullName)
    {
        if (!_localTypeCache.TryGetValue(fullName, out var result))
        {
            result = _localTypeCache[fullName] = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullName);
        }
        return result;
    }

    public TypeDefinition? FindReferenceTypeDefinition(string fullName)
    {
        if (!_referenceTypeCache.TryGetValue(fullName, out var result))
        {
            result = _referenceTypeCache[fullName] = resolver.ResolveTypeInReferences(fullName);
        }
        return result;
    }

    public TypeDefinition? ResolveType(string fullName) => FindLocalTypeDefinition(fullName) ?? resolver.ResolveTypeInReferences(fullName);

    public TypeDefinition ResolveTypeOrThrow(string fullName)
    {
        var result = ResolveType(fullName);
        if (result == null)
        {
            throw new Exception($"Unable to resolve type {fullName}");
        }
        return result;
    }

    public MethodDefinition? ResolveMethod(string fullName)
    {
        var (_, typeFullName) = ParseMethodFullName(fullName);
        var type = ResolveType(typeFullName);
        return type?.Methods.FirstOrDefault(x => x.FullName == fullName);
    }

    public MethodDefinition ResolveMethodOrThrow(string fullName)
    {
        var result = ResolveMethod(fullName);
        if (result == null)
        {
            throw new Exception($"Unable to resolve method {fullName}");
        }
        return result;
    }

    private static (string, string) ParseMethodFullName(string fullName)
    {
        if (fullName.Contains("::") && fullName.Contains(' '))
        {
            var items = fullName.Split("::");
            if (items.Length != 2)
            {
                throw new ArgumentException($"Unable to parse method full name: '{fullName}'");
            }

            var items2 = items[0].Split(" ");
            if (items2.Length != 2)
            {
                throw new ArgumentException($"Unable to parse return type on method method name: '{items[0]}'");
            }

            return (items2[0], items2[1]);
        }

        throw new ArgumentException($"Unable to find separated character in method full name: '{fullName}'");
    }

    public bool IsChildOf(TypeDefinition child, string parentFullName)
    {
        var parentType = ResolveType(parentFullName);
        if (parentType == null)
        {
            Log.Production($"Unable to find parent type {parentFullName}", ConsoleColor.DarkRed);
            return false;
        }

        return IsChildOf(child, parentType);
    }

    public bool IsChildOf(TypeDefinition child, TypeDefinition parent)
    {
        while (child.BaseType != null && child.BaseType.FullName != parent.FullName)
        {
            if (child.BaseType == null)
            {
                return false;
            }

            if (child.BaseType.FullName == parent.FullName)
            {
                return true;
            }

            var ancestor = ResolveType(child.BaseType.FullName);
            if (ancestor == null)
            {
                Log.Production($"Unable to find ancestor type {child.BaseType.FullName}", ConsoleColor.DarkRed);
                return false;
            }

            child = ancestor;
        }

        return child.BaseType != null && child.BaseType.FullName == parent.FullName;
    }

    public MethodDefinition? FindNearestMethod(TypeDefinition type, Func<MethodDefinition, bool> filter)
    {
        var baseType = type;

        while (baseType != null)
        {
            var method = baseType.Methods.FirstOrDefault(filter);

            if (method != null)
            {
                return method;
            }

            if (baseType.BaseType == null)
            {
                return null;
            }

            baseType = ResolveType(baseType.BaseType.FullName);
        }

        return null;
    }

    public void ApplyRenaming(FieldDefinition field, string newName)
    {
        if (field.Name == newName)
        {
            return;
        }
        field.Name = newName;

        if (field.IsPrivate)
        {
            ReplaceFieldReferenceInType(field.DeclaringType, field);
            return;
        }
        foreach (var type in field.Module.Types)
        {
            ReplaceFieldReferenceInType(type, field);
        }
        if (field.IsAssembly)
        {
            return;
        }
        foreach (var type in field.Module.Assembly.MainModule.GetAllTypes())
        {
            ReplaceFieldReferenceInType(type, field);
        }
    }

    public void ApplyRenaming(FieldDefinition field, string newName, IEnumerable<AssemblyDefinition> assemblyDefinitions)
    {
        ApplyRenaming(field, newName);
        foreach (var assemblyDefinition in assemblyDefinitions)
        {
            foreach (var type in assemblyDefinition.MainModule.GetAllTypes())
            {
                ReplaceFieldReferenceInType(type, field);
            }
        }
    }

    private void ReplaceFieldReferenceInType(TypeDefinition type, FieldDefinition field)
    {
        foreach (var method in type.Methods.Where(x => x.HasBody))
        {
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.Operand is not FieldReference fieldReference || fieldReference != field) continue;
                var operandFieldType = ResolveType(fieldReference.DeclaringType.FullName);
                var operandField = operandFieldType?.Fields.FirstOrDefault(x => x.FullName == fieldReference.FullName);
                if (operandField == null) continue;
                instruction.Operand = field;
            }
        }
    }
}