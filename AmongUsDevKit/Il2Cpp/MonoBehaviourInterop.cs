using AmongUsDevKit.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace AmongUsDevKit.Il2Cpp;

internal sealed class MonoBehaviourInterop(InteropMaker interopMaker)
{
    private static readonly Log.PersonalLogger Logger = Log.CreateLogger(nameof(MonoBehaviourInterop));

    public void Run()
    {
        Log.Production(nameof(MonoBehaviourInterop), ConsoleColor.Cyan);
        var components = interopMaker.MainAssembly.MainModule.Types
            .Where(IsMonoBehaviour)
            .Select(GetMonoBehaviourData)
            .ToList();

        foreach (var component in components)
        {
            Log.Verbose($"MonoBehaviour found: {component.Type.FullName} (nearest awake method in {component.NearestAwakeMethod?.DeclaringType.Name ?? "nowhere"}) (serialized fields: {component.SerializedFields.Count})", ConsoleColor.Magenta);
        }

        DoSerializedFieldsInterop(components);
    }

    private void DoSerializedFieldsInterop(List<MonoBehaviourData> allComponents)
    {
        Logger.Log("Starting serialized fields interop...", ConsoleColor.Blue);
        var components = allComponents.Where(x => x.SerializedFields.Count > 0).ToList();
        if (components.Count == 0)
        {
            Log.Verbose("No serialized field found. Skipping interop", ConsoleColor.Yellow);
            return;
        }
        foreach (var component in components)
        {
            var deserializationFields = new Dictionary<FieldDefinition, FieldDefinition>();
            foreach (var serializedField in component.SerializedFields)
            {
                var injectableFieldName = serializedField.Name;
                RenameUsedField(serializedField);
                var injectableField = CreateIl2CppInjectableFieldBasedOnSerializedField(serializedField, injectableFieldName);
                component.Type.Fields.Add(injectableField);
                interopMaker.DoNotRename(injectableField);
                deserializationFields[serializedField] = injectableField;
            }
            var deserializationMethod = CreateDeserializationMethod(deserializationFields);
            component.Type.Methods.Add(deserializationMethod);
            var awakeMethod = FindOrCreateAwakeMethod(component);
            AddDeserializationMethodCallInTopOfAwakeMethod(awakeMethod, deserializationMethod);
        }
        CreateMonoBehavioursRegisterer(components);
    }

    private void AddDeserializationMethodCallInTopOfAwakeMethod(MethodDefinition awakeMethod, MethodDefinition deserializationMethod)
    {
        var il = awakeMethod.Body.GetILProcessor();
        var hasBody = awakeMethod.HasBody;
        il.Prepend([
            il.Create(OpCodes.Ldarg_0),
            il.Create(OpCodes.Call, interopMaker.MainAssembly.MainModule.ImportReference(deserializationMethod))
        ]);
        if (hasBody) return;
        il.Emit(OpCodes.Ret);
    }
    
    private MethodDefinition FindOrCreateAwakeMethod(MonoBehaviourData component)
    {
        var awakeMethod = component.NearestAwakeMethod;
        if (awakeMethod == null)
        {
            awakeMethod = CreateEmptyAwakeMethod(MethodAttributes.Private);
            component.Type.Methods.Add(awakeMethod);
            return awakeMethod;
        }
        if (awakeMethod.DeclaringType.FullName == component.Type.FullName)
        {
            return awakeMethod;
        }
        if (awakeMethod.IsPrivate)
        {
            awakeMethod.IsPrivate = false;
            awakeMethod.IsFamily = true;
        }
        if (!awakeMethod.IsVirtual)
        {
            awakeMethod.IsVirtual = true;
        }
        var attributes = MethodAttributes.Virtual | MethodAttributes.HideBySig;
        if (awakeMethod.IsFamily)
        {
            attributes |= MethodAttributes.Family;
        }
        awakeMethod = CreateEmptyAwakeMethod(attributes);
        component.Type.Methods.Add(awakeMethod);
        return awakeMethod;
    }
    
    private MethodDefinition CreateEmptyAwakeMethod(MethodAttributes attributes)
    {
        var awakeMethod = new MethodDefinition("Awake", attributes, interopMaker.MainAssembly.MainModule.TypeSystem.Void);

        var il = awakeMethod.Body.GetILProcessor();
        il.Emit(OpCodes.Ret);

        return awakeMethod;
    }

    private MethodDefinition CreateDeserializationMethod(Dictionary<FieldDefinition, FieldDefinition> deserializationFields)
    {
        var method = new MethodDefinition(RandomProvider.CreateRandomMethodName(), MethodAttributes.Private, interopMaker.MainAssembly.MainModule.TypeSystem.Void);
        var il = method.Body.GetILProcessor();

        foreach (var (serializedField, il2CppField) in deserializationFields)
        {
            var il2CppType = GetInteropFieldTypeDefinition(serializedField);
            var genericIl2CppType = interopMaker.MainAssembly.MainModule.ImportReference(il2CppType.MakeGenericInstanceType(serializedField.FieldType));
            var il2CppGetMethod = interopMaker.MainAssembly.MainModule.ImportReference(il2CppType.Methods.First(x => x.Name == "Get"));
            il2CppGetMethod.DeclaringType = genericIl2CppType;
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, il2CppField);
            il.Emit(OpCodes.Callvirt, il2CppGetMethod);
            il.Emit(OpCodes.Stfld, serializedField);
        }
        
        il.Emit(OpCodes.Ret);

        return method;
    }

    private void RenameUsedField(FieldDefinition serializedField)
    {
        var oldName = serializedField.FullName;
        var newName = RandomProvider.CreateRandomFieldName();
        interopMaker.Helper.ApplyRenaming(serializedField, newName);
        Log.Verbose($"Renamed field from {oldName} to {serializedField.FullName}", ConsoleColor.DarkCyan);
    }

    private FieldDefinition CreateIl2CppInjectableFieldBasedOnSerializedField(FieldDefinition serializedField, string injectableFieldName)
    {
        var injectableField = new FieldDefinition(injectableFieldName, FieldAttributes.Public, interopMaker.MainAssembly.MainModule.ImportReference(GetInteropFieldTypeReference(serializedField)));

        return injectableField;
    }

    private TypeReference GetInteropFieldTypeReference(FieldDefinition field)
    {
        var type = GetInteropFieldTypeDefinition(field);
        if (type.HasGenericParameters)
        {
            return type.CreateGenericType(field.FieldType);
        }
        return type;
    }

    private TypeDefinition GetInteropFieldTypeDefinition(FieldDefinition field)
    {
        var type = interopMaker.Helper.ResolveTypeOrThrow(field.FieldType.FullName);
        if (type.FullName == interopMaker.MainAssembly.MainModule.TypeSystem.String.FullName)
        {
            var il2CppStringFieldType = interopMaker.Helper.ResolveTypeOrThrow("Il2CppInterop.Runtime.InteropTypes.Fields.Il2CppStringField");
            return il2CppStringFieldType;
        }
        if (interopMaker.Helper.IsChildOf(type, "UnityEngine.Object"))
        {
            var il2CppReferenceFieldType = interopMaker.Helper.ResolveTypeOrThrow("Il2CppInterop.Runtime.InteropTypes.Fields.Il2CppReferenceField`1");
            return il2CppReferenceFieldType;
        }
        var il2CppValueFieldType = interopMaker.Helper.ResolveTypeOrThrow("Il2CppInterop.Runtime.InteropTypes.Fields.Il2CppValueField`1");
        return il2CppValueFieldType;
    }

    private void CreateMonoBehavioursRegisterer(List<MonoBehaviourData> components)
    {
        var classInjectorType = interopMaker.Helper.FindReferenceTypeDefinition("Il2CppInterop.Runtime.Injection.ClassInjector");
        if (classInjectorType == null)
        {
            throw new Exception("Unable to find ClassInjector type");
        }
        var isRegisteredMethod = classInjectorType.Methods.First(x => x.Name == "IsTypeRegisteredInIl2Cpp" && x.HasGenericParameters);
        var registerMethod = classInjectorType.Methods.First(x => x.Name == "RegisterTypeInIl2Cpp" && x.HasGenericParameters);

        var registerMonoBehavioursInIl2CppMethod = new MethodDefinition(
            RandomProvider.CreateRandomMethodName(),
            MethodAttributes.Static | MethodAttributes.Private,
            interopMaker.MainAssembly.MainModule.TypeSystem.Void
        );

        var il = registerMonoBehavioursInIl2CppMethod.Body.GetILProcessor();

        var nextBranch = il.Create(OpCodes.Ret);
        il.Append(nextBranch);

        interopMaker.CompilerType.Methods.Add(registerMonoBehavioursInIl2CppMethod);

        for (var i = components.Count - 1; i >= 0; i--)
        {
            var type = components[i].Type;
            var conditionMethod = new GenericInstanceMethod(isRegisteredMethod);
            conditionMethod.GenericArguments.Add(type);
            var actionMethod = new GenericInstanceMethod(registerMethod);
            actionMethod.GenericArguments.Add(type);

            var nextInstruction = nextBranch;
            nextBranch = il.Create(OpCodes.Call, interopMaker.MainAssembly.MainModule.ImportReference(conditionMethod));
            il.Prepend([
                nextBranch,
                il.Create(OpCodes.Brtrue_S, nextInstruction),
                il.CreateCallMethod(interopMaker.MainAssembly.MainModule.ImportReference(actionMethod))
            ]);
        }

        interopMaker.BaseLoader.AddCallBeforeReturn(interopMaker.MainAssembly.MainModule.ImportReference(registerMonoBehavioursInIl2CppMethod));
    }

    private MonoBehaviourData GetMonoBehaviourData(TypeDefinition type) => new(
        type,
        interopMaker.Helper.FindNearestMethod(type, method => method.Name == "Awake"),
        type.Fields.Where(IsUnitySerializedField).ToList()
    );

    private bool IsMonoBehaviour(TypeDefinition type) => interopMaker.Helper.IsChildOf(type, "UnityEngine.MonoBehaviour");

    private static bool IsUnitySerializedField(FieldDefinition field)
    {
        if (field.IsLiteral || field.IsStatic || field.IsPrivate || field.IsFamilyOrAssembly || field.IsInitOnly) return false;
        return (!field.HasCustomAttributes || !field.HasCustomAttribute("System.NonSerializedAttribute")) && field.IsPublic;
    }

    private sealed class MonoBehaviourData(TypeDefinition type, MethodDefinition? nearestAwakeMethod, List<FieldDefinition> serializedFields)
    {
        public readonly TypeDefinition Type = type;
        public readonly MethodDefinition? NearestAwakeMethod = nearestAwakeMethod;
        public readonly List<FieldDefinition> SerializedFields = serializedFields;
    }
}