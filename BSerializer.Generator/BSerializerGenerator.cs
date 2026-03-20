using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BSerializer.Generator;

[Generator]
public class BSerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contextClasses = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "BSerializer.BSerializableAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => ExtractModel(ctx))
            .Where(static m => m != null)
            .Select(static (m, _) => m!);

        context.RegisterSourceOutput(contextClasses.Collect(), static (spc, models) =>
        {
            // Group by context class (multiple attributes on same class)
            var grouped = models.GroupBy(m => m.ContextFullName).ToList();
            foreach (var group in grouped)
            {
                var first = group.First();
                var allRootTypes = group.SelectMany(m => m.RootTypes).Distinct().ToList();
                var combined = new ContextModel(first.ContextName, first.ContextFullName, first.ContextNamespace, allRootTypes);
                var source = CodeEmitter.Emit(combined, spc);
                spc.AddSource($"{first.ContextName}.g.cs", source);
            }
        });
    }

    private static ContextModel? ExtractModel(GeneratorAttributeSyntaxContext ctx)
    {
        if (ctx.TargetSymbol is not INamedTypeSymbol contextSymbol)
            return null;

        var rootTypes = new List<INamedTypeSymbol>();

        foreach (var attr in contextSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name != "BSerializableAttribute") continue;
            if (attr.ConstructorArguments.Length != 1) continue;
            if (attr.ConstructorArguments[0].Value is not INamedTypeSymbol typeArg) continue;
            rootTypes.Add(typeArg);
        }

        if (rootTypes.Count == 0) return null;

        return new ContextModel(
            contextSymbol.Name,
            contextSymbol.ToDisplayString(),
            contextSymbol.ContainingNamespace.ToDisplayString(),
            rootTypes);
    }
}

internal class ContextModel
{
    public string ContextName { get; }
    public string ContextFullName { get; }
    public string ContextNamespace { get; }
    public List<INamedTypeSymbol> RootTypes { get; }

    public ContextModel(string name, string fullName, string ns, List<INamedTypeSymbol> rootTypes)
    {
        ContextName = name;
        ContextFullName = fullName;
        ContextNamespace = ns;
        RootTypes = rootTypes;
    }
}
