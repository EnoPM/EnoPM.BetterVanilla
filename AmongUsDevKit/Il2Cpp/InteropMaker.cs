using AmongUsDevKit.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AmongUsDevKit.Il2Cpp;

internal sealed class InteropMaker
{
    private const string CompilerTypesNamespace = "AmongUsDevKit.LibraryRuntime";

    public readonly AssemblyDefinition MainAssembly;
    public readonly AmongUsPluginResolver Resolver;
    public readonly AmongUsReferenceHelper Helper;
    public readonly TypeDefinition CompilerType;
    public readonly MethodDefinition BaseLoader;

    private MethodDefinition? _doNotRenameAttributeConstructor;

    internal InteropMaker(string inputPath)
    {
        Resolver = new AmongUsPluginResolver();
        MainAssembly = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters
        {
            ReadingMode = ReadingMode.Immediate,
            ReadWrite = false,
            InMemory = true,
            AssemblyResolver = Resolver
        });
        Helper = new AmongUsReferenceHelper(MainAssembly, Resolver);
        CompilerType = CreateInternalStaticCompilerClass("MainInterop");
        BaseLoader = CreateBaseLoader();
    }

    public void LoadDependencies() => Resolver.LoadLibraries(MainAssembly);

    public void RegisterPluginEntryPoint()
    {
        var pluginEntryPointType = MainAssembly.MainModule.Types.FirstOrDefault(x => Helper.IsChildOf(x, "BepInEx.Unity.IL2CPP.BasePlugin"));
        if (pluginEntryPointType == null)
        {
            throw new Exception("Unable to find plugin entry point type");
        }
        var pluginEntryPointMethod = pluginEntryPointType.Methods.FirstOrDefault(x => x.IsVirtual && x.IsReuseSlot && x.Name == "Load");
        if (pluginEntryPointMethod == null)
        {
            throw new Exception($"Unable to find plugin entry point method in {pluginEntryPointType.FullName}");
        }
        pluginEntryPointMethod.AddCallOnTop(MainAssembly.MainModule.ImportReference(BaseLoader));

        var attribute = CreateAttributeClass("CanNotBeObfuscate", AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Event);
        _doNotRenameAttributeConstructor = attribute.Methods.First(x => x.IsConstructor);
    }

    public void RemoveAssemblyNameSuffix(string suffix)
    {
        if (!MainAssembly.Name.Name.EndsWith(suffix)) return;
        MainAssembly.Name.Name = MainAssembly.Name.Name[..^suffix.Length];
    }

    public void RandomizeAssemblyVersion()
    {
        MainAssembly.Name.Version = RandomProvider.CreateRandomVersion();
    }

    public void Save(string outputPath) => MainAssembly.Write(outputPath);

    public void DoNotRename(IMemberDefinition member)
    {
        if (_doNotRenameAttributeConstructor == null)
        {
            throw new Exception($"{nameof(InteropMaker)} must be initialized");
        }
        if (member.CustomAttributes.Any(x => x.AttributeType.FullName == _doNotRenameAttributeConstructor.DeclaringType.FullName)) return;
        member.CustomAttributes.Add(new CustomAttribute(MainAssembly.MainModule.ImportReference(_doNotRenameAttributeConstructor)));
    }

    public TypeDefinition CreateCompilerType(string typeName, TypeAttributes attributes, TypeReference baseType)
    {
        var type = new TypeDefinition(CompilerTypesNamespace, typeName, attributes, baseType);
        MainAssembly.MainModule.Types.Add(type);

        return type;
    }

    public TypeDefinition CreateInternalStaticCompilerClass(string className) => CreateCompilerType(className, TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.NotPublic, MainAssembly.MainModule.TypeSystem.Object);


    private const TypeAttributes AttributeTypeBaseAttributes = TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.Sealed;
    private const MethodAttributes AttributeConstructorBaseAttributes = MethodAttributes.Assembly | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

    public TypeDefinition CreateAttributeClass(string attributeName, AttributeTargets attributeTargets)
    {
        var mainModule = MainAssembly.MainModule;
        var attributeBaseClass = Helper.ResolveTypeOrThrow("System.Attribute");
        var attributeBaseCtor = attributeBaseClass.Methods.First(x => x.IsConstructor);
        var attributeUsageCtor = Helper.ResolveMethodOrThrow("System.Void System.AttributeUsageAttribute::.ctor(System.AttributeTargets)");
        var attributeTargetsType = Helper.ResolveTypeOrThrow("System.AttributeTargets");
        
        var attributeType = CreateCompilerType(attributeName, AttributeTypeBaseAttributes, mainModule.ImportReference(attributeBaseClass));

        var ctor = new MethodDefinition(".ctor", AttributeConstructorBaseAttributes, mainModule.TypeSystem.Void);
        
        var attributeUsage = new CustomAttribute(mainModule.ImportReference(attributeUsageCtor));
        attributeUsage.ConstructorArguments.Add(new CustomAttributeArgument(mainModule.ImportReference(attributeTargetsType), attributeTargets));
    
        attributeType.CustomAttributes.Add(attributeUsage);
        attributeType.Methods.Add(ctor);

        var il = ctor.Body.GetILProcessor();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, mainModule.ImportReference(attributeBaseCtor));
        il.Emit(OpCodes.Ret);

        return attributeType;
    }

    private MethodDefinition CreateBaseLoader()
    {
        var method = new MethodDefinition(RandomProvider.CreateRandomMethodName(), MethodAttributes.Static | MethodAttributes.Assembly, MainAssembly.MainModule.TypeSystem.Void);

        var il = method.Body.GetILProcessor();
        il.Emit(OpCodes.Ret);

        CompilerType.Methods.Add(method);

        return method;
    }
}