using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BSerializer.Generator;

internal class TypeInfo
{
    public uint Id { get; set; }
    public string FullName { get; set; } = "";
    public string ShortName { get; set; } = "";
    public string SafeName { get; set; } = ""; // for method names
    public List<PropInfo> Properties { get; set; } = new();
}

internal class PropInfo
{
    public uint Id { get; set; }
    public uint TypeId { get; set; }
    public string Name { get; set; } = "";
    public ITypeSymbol Type { get; set; } = null!;
    public TypeCategory Category { get; set; }
}

internal enum TypeCategory
{
    Primitive,
    ByteArray,
    Array,
    List,
    Dictionary,
    NullableValue,
    CustomObject
}

internal class TypeAnalyzer
{
    private readonly List<TypeInfo> _customTypes = new();
    private readonly HashSet<string> _visited = new();
    private uint _nextId = 16; // primitives use 1-15

    private static readonly Dictionary<SpecialType, uint> PrimitiveIds = new()
    {
        [SpecialType.System_Boolean] = 1,
        [SpecialType.System_Byte] = 2,
        [SpecialType.System_SByte] = 3,
        [SpecialType.System_Int16] = 4,
        [SpecialType.System_UInt16] = 5,
        [SpecialType.System_Int32] = 6,
        [SpecialType.System_UInt32] = 7,
        [SpecialType.System_Int64] = 8,
        [SpecialType.System_UInt64] = 9,
        [SpecialType.System_Single] = 10,
        [SpecialType.System_Double] = 11,
        [SpecialType.System_Decimal] = 12,
        [SpecialType.System_Char] = 13,
        [SpecialType.System_String] = 14,
    };

    public List<TypeInfo> CustomTypes => _customTypes;

    public void AnalyzeRootType(INamedTypeSymbol type)
    {
        GetTypeId(type);
    }

    // Replicates InternalSerializer.GetTypeId depth-first traversal
    private uint GetTypeId(ITypeSymbol type)
    {
        type = type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var fullName = type.ToDisplayString();

        // byte[] special case
        if (type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte)
            return 15; // ByteArray primitive

        if (PrimitiveIds.TryGetValue(type.SpecialType, out var primId))
            return primId;

        if (type is IArrayTypeSymbol arrayType)
        {
            GetTypeId(arrayType.ElementType);
            return 0;
        }

        if (type is INamedTypeSymbol named && named.IsGenericType)
        {
            var genericDef = named.ConstructedFrom.ToDisplayString();
            if (genericDef == "System.Collections.Generic.List<T>")
            {
                GetTypeId(named.TypeArguments[0]);
                return 0;
            }
            if (genericDef == "System.Collections.Generic.Dictionary<TKey, TValue>")
            {
                GetTypeId(named.TypeArguments[0]);
                GetTypeId(named.TypeArguments[1]);
                return 0;
            }
            if (genericDef == "System.Nullable<T>")
            {
                GetTypeId(named.TypeArguments[0]);
                return 0; // Nullable<T> treated specially
            }
        }

        if (_visited.Contains(fullName))
            return _customTypes.First(t => t.FullName == fullName).Id;

        var info = CreateTypeInfo(type);
        return info.Id;
    }

    private TypeInfo CreateTypeInfo(ITypeSymbol type)
    {
        type = type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var fullName = type.ToDisplayString();
        var id = _nextId++;

        var info = new TypeInfo
        {
            Id = id,
            FullName = fullName,
            ShortName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            SafeName = fullName.Replace(".", "_").Replace("<", "_").Replace(">", "_").Replace(",", "_").Replace(" ", "").Replace("?", "")
        };

        _customTypes.Add(info);
        _visited.Add(fullName);

        // Collect properties in reflection-compatible order:
        // declared on derived class first, then base class
        var properties = CollectProperties(type);
        uint propId = 0;
        foreach (var prop in properties)
        {
            var propTypeId = GetTypeId(prop.Type);
            info.Properties.Add(new PropInfo
            {
                Id = propId++,
                TypeId = propTypeId,
                Name = prop.Name,
                Type = prop.Type,
                Category = CategorizeType(prop.Type)
            });
        }

        return info;
    }

    // Matches Type.GetProperties(BindingFlags.Public | BindingFlags.Instance) order:
    // derived class properties first, then base class
    private static List<IPropertySymbol> CollectProperties(ITypeSymbol type)
    {
        var result = new List<IPropertySymbol>();
        var current = type;
        while (current != null && current.SpecialType != SpecialType.System_Object)
        {
            var declared = current.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public
                            && !p.IsStatic
                            && !p.IsIndexer
                            && p.GetMethod != null
                            && p.SetMethod != null)
                .ToList();
            result.AddRange(declared);
            current = current.BaseType;
        }
        return result;
    }

    internal static TypeCategory CategorizeType(ITypeSymbol type)
    {
        var fullName = type.ToDisplayString();

        if (type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte)
            return TypeCategory.ByteArray;

        if (PrimitiveIds.ContainsKey(type.SpecialType))
            return TypeCategory.Primitive;

        if (type is IArrayTypeSymbol)
            return TypeCategory.Array;

        if (type is INamedTypeSymbol named && named.IsGenericType)
        {
            var genericDef = named.ConstructedFrom.ToDisplayString();
            if (genericDef == "System.Collections.Generic.List<T>")
                return TypeCategory.List;
            if (genericDef == "System.Collections.Generic.Dictionary<TKey, TValue>")
                return TypeCategory.Dictionary;
            if (genericDef == "System.Nullable<T>")
                return TypeCategory.NullableValue;
        }

        return TypeCategory.CustomObject;
    }

    internal static bool IsReferenceType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol named && named.IsGenericType
            && named.ConstructedFrom.ToDisplayString() == "System.Nullable<T>")
            return true; // treat Nullable<T> as "needs null prefix"

        return !type.IsValueType;
    }
}
