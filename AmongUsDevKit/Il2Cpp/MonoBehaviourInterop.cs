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

        var allMonoBehaviours = GetMonoBehavioursOrderedByInheritance();

        var components = new List<MonoBehaviourData>();

        foreach (var monoBehaviourType in allMonoBehaviours)
        {
            components.Add(GetMonoBehaviourData(monoBehaviourType));
        }
        
        //IdentifyDependencies(components);
        //IdentifyMethodDependencies(components);
        //components = TopologicalSort(components);

        foreach (var component in components)
        {
            Log.Verbose($"MonoBehaviour found: {component.Type.FullName} (serialized fields: {component.SerializedFields.Count})", ConsoleColor.Magenta);
        }

        DoSerializedFieldsInterop(components);
        HideUnsupportedTypeInIl2Cpp(components);
        CreateMonoBehavioursRegisterer(components);
        RemoveAbstractItems(components);
    }

    private void RemoveAbstractItems(List<MonoBehaviourData> allComponents)
    {
        foreach (var component in allComponents)
        {
            var type = component.Type;
            foreach (var method in type.Methods)
            {
                if (!method.IsAbstract) continue;
                ConvertAbstractMethodToVirtual(method);
            }
        }
    }

    private void ConvertAbstractMethodToVirtual(MethodDefinition method)
    {
        method.IsAbstract = false;
        method.IsVirtual = true;

        var notImplementedExceptionConstructor = interopMaker.Helper.ResolveMethodOrThrow("System.Void System.NotImplementedException::.ctor()");
        
        var il = method.Body.GetILProcessor();
        il.Emit(OpCodes.Newobj, interopMaker.MainAssembly.MainModule.ImportReference(notImplementedExceptionConstructor));
        il.Emit(OpCodes.Throw);
    }

    private void HideUnsupportedTypeInIl2Cpp(List<MonoBehaviourData> allComponents)
    {
        var unsupportedMembers = new List<IMemberDefinition>();
        foreach (var component in allComponents)
        {
            foreach (var method in component.Type.Methods)
            {
                if (IsTypeSupportedInIl2Cpp(method.ReturnType) && method.Parameters.All(x => IsTypeSupportedInIl2Cpp(x.ParameterType)))
                {
                    continue;
                }
                unsupportedMembers.Add(method);
            }
            foreach (var ev in component.Type.Events)
            {
                if (IsTypeSupportedInIl2Cpp(ev.EventType))
                {
                    continue;
                }
                unsupportedMembers.Add(ev);
            }
            foreach (var property in component.Type.Properties)
            {
                if (IsTypeSupportedInIl2Cpp(property.PropertyType))
                {
                    continue;
                }
                unsupportedMembers.Add(property);
            }
        }
        if (unsupportedMembers.Count == 0) return;
        var hideFromIl2CppAttributeConstructor = interopMaker.Helper.ResolveMethodOrThrow("System.Void Il2CppInterop.Runtime.Attributes.HideFromIl2CppAttribute::.ctor()");
        foreach (var unsupportedMember in unsupportedMembers)
        {
            unsupportedMember.CustomAttributes.Add(new CustomAttribute(interopMaker.MainAssembly.MainModule.ImportReference(hideFromIl2CppAttributeConstructor)));
        }
    }

    private bool IsTypeSupportedInIl2Cpp(TypeReference type)
    {
        if (type.IsValueType || type.FullName == interopMaker.MainAssembly.MainModule.TypeSystem.Void.FullName || type.FullName == interopMaker.MainAssembly.MainModule.TypeSystem.String.FullName || type.IsGenericParameter)
        {
            return true;
        }

        if (type.IsByReference)
        {
            return IsTypeSupportedInIl2Cpp(((ByReferenceType)type).ElementType);
        }
        
        var resolvedType = interopMaker.Helper.ResolveType(type.FullName);
        if (resolvedType == null)
        {
            return false;
        }

        if (interopMaker.Helper.IsChildOf(resolvedType, "Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase"))
        {
            return resolvedType.Module.Assembly != interopMaker.MainAssembly;
        }

        return false;
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
            var nearestAwakeMethod = interopMaker.Helper.FindNearestMethod(component.Type, method => method.Name == "Awake");
            Log.Verbose($"[{component.Type.Name}]: Nearest awake method -> {nearestAwakeMethod?.FullName ?? "unknown"}");
            var awakeMethod = FindOrCreateAwakeMethod(component, nearestAwakeMethod, out var parentAwakeMethod);
            AddDeserializationMethodCallInTopOfAwakeMethod(awakeMethod, deserializationMethod, parentAwakeMethod);
        }
    }

    private void AddDeserializationMethodCallInTopOfAwakeMethod(MethodDefinition awakeMethod, MethodDefinition deserializationMethod, MethodDefinition? parentAwakeMethod)
    {
        var il = awakeMethod.Body.GetILProcessor();
        il.Prepend([
            il.Create(OpCodes.Ldarg_0),
            il.Create(OpCodes.Call, interopMaker.MainAssembly.MainModule.ImportReference(deserializationMethod))
        ]);
        if (parentAwakeMethod != null && !awakeMethod.Body.Instructions.Any(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference xMethod && xMethod.FullName == parentAwakeMethod.FullName))
        {
            il.Prepend([
                il.Create(OpCodes.Ldarg_0),
                il.Create(OpCodes.Call, interopMaker.MainAssembly.MainModule.ImportReference(parentAwakeMethod))
            ]);
        }
    }

    private MethodDefinition? EnsureParentAwakeMethodAccess(TypeDefinition type)
    {
        var parentAwakeMethod = interopMaker.Helper.FindNearestMethod(type, method => method.DeclaringType.FullName != type.FullName && method.Name == "Awake");
        if (parentAwakeMethod == null) return null;
        if (parentAwakeMethod.IsPrivate)
        {
            parentAwakeMethod.IsPrivate = false;
            parentAwakeMethod.IsFamily = true;
        }
        if (!parentAwakeMethod.IsVirtual)
        {
            parentAwakeMethod.IsVirtual = true;
        }

        return parentAwakeMethod;
    }
    
    private MethodDefinition FindOrCreateAwakeMethod(MonoBehaviourData component, MethodDefinition? nearestAwakeMethod, out MethodDefinition? parentAwakeMethodResult)
    {
        Log.Verbose($"[{component.Type.Name}] {nameof(FindOrCreateAwakeMethod)} {nearestAwakeMethod?.FullName}");
        // No awake method found in parent or in current class
        if (nearestAwakeMethod == null)
        {
            var newAwakeMethod = CreateEmptyAwakeMethod(MethodAttributes.Private);
            component.Type.Methods.Add(newAwakeMethod);
            Log.Verbose($"|||| [{component.Type.Name}] {nameof(FindOrCreateAwakeMethod)} {nearestAwakeMethod?.FullName} {newAwakeMethod.FullName}");
            parentAwakeMethodResult = null;
            return newAwakeMethod;
        }
        // Awake method found in current class
        if (nearestAwakeMethod.DeclaringType.FullName == component.Type.FullName)
        {
            var parentAwakeMethod = EnsureParentAwakeMethodAccess(component.Type);
            // Parent class has a generated Awake method. So we need to make current Awake an override and ensure parent Awake method is call in child Awake method override.
            if (parentAwakeMethod != null)
            {
                nearestAwakeMethod.IsPrivate = false;
                nearestAwakeMethod.IsPublic = parentAwakeMethod.IsPublic;
                nearestAwakeMethod.IsFamily = parentAwakeMethod.IsFamily;
                nearestAwakeMethod.IsVirtual = true;
                nearestAwakeMethod.IsHideBySig = true;
            }
            parentAwakeMethodResult = parentAwakeMethod;
            Log.Verbose($"|||| [{component.Type.Name}] {nameof(FindOrCreateAwakeMethod)} {nearestAwakeMethod.FullName} {nearestAwakeMethod.FullName}");
            return nearestAwakeMethod;
        }
        // Awake method found in parent class
        if (nearestAwakeMethod.IsPrivate)
        {
            nearestAwakeMethod.IsPrivate = false;
            nearestAwakeMethod.IsFamily = true;
        }
        if (!nearestAwakeMethod.IsVirtual)
        {
            nearestAwakeMethod.IsVirtual = true;
        }
        var attributes = MethodAttributes.Virtual | MethodAttributes.HideBySig;
        if (nearestAwakeMethod.IsFamily)
        {
            attributes |= MethodAttributes.Family;
        }
        var awakeMethod = CreateEmptyAwakeMethod(attributes);
        component.Type.Methods.Add(awakeMethod);
        
        Log.Verbose($"|||| [{component.Type.Name}] {nameof(FindOrCreateAwakeMethod)} {nearestAwakeMethod?.FullName} {awakeMethod.FullName}");
        parentAwakeMethodResult = nearestAwakeMethod;
        
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
            Log.Verbose($"[{nameof(CreateDeserializationMethod)}] ==> {il2CppType.FullName} {il2CppType.HasGenericParameters}");
            var il2CppGetMethod = interopMaker.MainAssembly.MainModule.ImportReference(il2CppType.Methods.First(x => x.Name == "Get"));
            if (il2CppType.HasGenericParameters)
            {
                il2CppGetMethod.DeclaringType = interopMaker.MainAssembly.MainModule.ImportReference(il2CppType.MakeGenericInstanceType(serializedField.FieldType));
            }
            
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
        var type = interopMaker.Helper.ResolveTypeOrThrow(field.FieldType);
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
            Log.Verbose($"Add registration call for component {type.FullName}");
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
        type.Fields.Where(IsUnitySerializedField).ToList()
    );
    
    private void IdentifyDependencies(List<MonoBehaviourData> components)
    {
        foreach (var component in components)
        {
            foreach (var field in component.Type.Fields)
            {
                var fieldType = interopMaker.Helper.ResolveType(field.FieldType.FullName);
                if (fieldType != null && components.Any(c => c.Type == fieldType))
                {
                    component.Dependencies.Add(fieldType);
                }
            }

            foreach (var method in component.Type.Methods)
            {
                foreach (var parameter in method.Parameters)
                {
                    var paramType = interopMaker.Helper.ResolveType(parameter.ParameterType.FullName);
                    if (paramType != null && components.Any(c => c.Type == paramType))
                    {
                        component.Dependencies.Add(paramType);
                    }
                }
            }
        }
    }
    
    private void IdentifyMethodDependencies(List<MonoBehaviourData> components)
    {
        foreach (var component in components)
        {
            foreach (var method in component.Type.Methods)
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                    {
                        var methodRef = instruction.Operand as MethodReference;
                        if (methodRef == null) continue;
                        
                        if (methodRef.Name == "AddComponent" && methodRef.DeclaringType.FullName == "UnityEngine.GameObject")
                        {
                            var genericInstanceMethod = methodRef as GenericInstanceMethod;
                            if (genericInstanceMethod != null)
                            {
                                var dependencyType = genericInstanceMethod.GenericArguments[0].Resolve();
                                if (components.Any(c => c.Type == dependencyType))
                                {
                                    component.Dependencies.Add(dependencyType);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    private List<TypeDefinition> GetMonoBehavioursOrderedByInheritance()
    {
        var types = interopMaker.MainAssembly.MainModule.Types.Where(IsMonoBehaviour).ToList();
        var sortedList = new List<TypeDefinition>();
        var visited = new HashSet<TypeDefinition>();

        foreach (var type in types)
        {
            VisitType(type, types, visited, sortedList);
        }

        sortedList.Reverse();

        return sortedList;
    }
    
    private void VisitType(
        TypeDefinition type, 
        List<TypeDefinition> allTypes, 
        HashSet<TypeDefinition> visited, 
        List<TypeDefinition> sortedList)
    {
        if (!visited.Add(type)) return;
        foreach (var potentialChild in allTypes)
        {
            if (interopMaker.Helper.IsChildOf(potentialChild, type))
            {
                VisitType(potentialChild, allTypes, visited, sortedList); // Visiter les enfants avant d'ajouter le parent
            }
        }

        sortedList.Add(type);
    }

    
    private static List<MonoBehaviourData> TopologicalSort(List<MonoBehaviourData> components)
    {
        var sorted = new List<MonoBehaviourData>();
        var visited = new HashSet<MonoBehaviourData>();

        void Visit(MonoBehaviourData component)
        {
            if (!visited.Contains(component))
            {
                visited.Add(component);
                foreach (var dependencyType in component.Dependencies)
                {
                    var dependency = components.FirstOrDefault(c => c.Type == dependencyType);
                    if (dependency != null)
                    {
                        Visit(dependency);
                    }
                }
                sorted.Add(component);
            }
        }

        foreach (var component in components)
        {
            Visit(component);
        }

        return sorted;
    }

    private bool IsMonoBehaviour(TypeDefinition type) => interopMaker.Helper.IsChildOf(type, "UnityEngine.MonoBehaviour");

    private static bool IsUnitySerializedField(FieldDefinition field)
    {
        if (field.IsLiteral || field.IsStatic || field.IsPrivate || field.IsFamilyOrAssembly || field.IsInitOnly) return false;
        if (field.HasCustomAttributes && field.HasCustomAttribute("AmongUsDevKit.Api.DoNotInteropThisFieldAttribute")) return false;
        return (!field.HasCustomAttributes || !field.HasCustomAttribute("System.NonSerializedAttribute")) && field.IsPublic;
    }

    private sealed class MonoBehaviourData(TypeDefinition type, List<FieldDefinition> serializedFields)
    {
        public readonly TypeDefinition Type = type;
        public readonly List<FieldDefinition> SerializedFields = serializedFields;
        public readonly List<TypeDefinition> Dependencies = [];
    }
}