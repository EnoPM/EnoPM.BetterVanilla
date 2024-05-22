using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EnoPM.Generators;

[Generator]
public class ManagedMonoBehaviourGenerator : ISourceGenerator
{


    public void Initialize(GeneratorInitializationContext context)
    {

    }

    private static string GetNamespace(ClassDeclarationSyntax classDeclaration)
    {
        var encapsulateNamespace = classDeclaration.AncestorsAndSelf()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault();
        if (encapsulateNamespace != null)
        {
            return encapsulateNamespace.Name.ToString();
        }
        var classRoot = classDeclaration.SyntaxTree.GetRoot();
        if (classRoot is CompilationUnitSyntax compilationUnit)
        {
            var fileScopedNamespace = compilationUnit.Members.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
            if (fileScopedNamespace != null)
            {
                return fileScopedNamespace.Name.ToString();
            }
        }
        return string.Empty;
    }

    private static string GeneratePartialClass(ClassDeclarationSyntax baseClassDeclaration, List<FieldDeclarationSyntax> baseFieldDeclarations)
    {
        var baseNamespace = GetNamespace(baseClassDeclaration);

        if (baseNamespace == string.Empty)
        {
            return string.Empty;
        }

        var statements = new List<StatementSyntax>();
        var fields = new List<MemberDeclarationSyntax>();
        foreach (var baseField in baseFieldDeclarations)
        {
            var genericType = GenericName(Identifier("Il2CppReferenceField"))
                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(baseField.Declaration.Type)));
            var field = FieldDeclaration(
                    VariableDeclaration(genericType)
                        .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier($"{GeneratedFieldPrefix}{baseField.Declaration.Variables.First().Identifier.Text}{GeneratedFieldSuffix}"))))
                )
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            fields.Add(field);
            var assignment = AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(baseField.Declaration.Variables.First().Identifier.Text),
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(field.Declaration.Variables.First().Identifier.Text),
                        IdentifierName("Get"))).WithArgumentList(ArgumentList()));
            statements.Add(ExpressionStatement(assignment)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }

        var awakeMethod = MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier("Awake"))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PrivateKeyword)))
            .WithBody(Block(statements));

        var @class = ClassDeclaration(baseClassDeclaration.Identifier.Text)
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddMembers(fields.ToArray())
            .AddMembers(awakeMethod);

        var @namespace = NamespaceDeclaration(ParseName(baseNamespace))
            .WithMembers(SingletonList<MemberDeclarationSyntax>(@class));

        var compilationUnit = CompilationUnit()
            .WithUsings(
                List(
                    new[]
                    {
                        UsingDirective(ParseName("Il2CppInterop.Runtime.InteropTypes.Fields")), UsingDirective(ParseName("UnityEngine")), UsingDirective(ParseName("UnityEngine.UI")), UsingDirective(ParseName("TMPro")),
                    }
                )
            )
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(@namespace)
            );

        return compilationUnit.NormalizeWhitespace().ToFullString();
    }

    private const string FieldAttributeName = "ManagedByEditor";
    private readonly List<string> _monoBehaviourParents = ["MonoBehaviour"];
    private const string GeneratedFieldPrefix = "";
    private const string GeneratedFieldSuffix = "Ref";

    public void Execute(GeneratorExecutionContext context)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "GEN0001",
                title: "Code generator message",
                messageFormat: "Generating managed MonoBehaviour code for {0}",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            Location.None,
            context.Compilation.Assembly.Name));
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var classDeclarations = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations)
            {
                if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
                var baseType = classDeclaration.BaseList?.Types.FirstOrDefault(x => _monoBehaviourParents.Contains(x.ToString()));
                if (baseType == null) continue;
                var fieldsToGenerate = new List<FieldDeclarationSyntax>();
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "GEN0001",
                        title: "Code generator message",
                        messageFormat: "Lookup MonoBehaviour type {0}",
                        category: "Generation",
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    Location.None,
                    classDeclaration.Identifier.Text));

                var fields = classDeclaration.DescendantNodes()
                    .OfType<FieldDeclarationSyntax>();

                foreach (var field in fields)
                {
                    if (field.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword))) continue;
                    if (!field.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == FieldAttributeName))) continue;
                    fieldsToGenerate.Add(field);
                }

                if (fieldsToGenerate.Count == 0) continue;
                var sourceCode = GeneratePartialClass(classDeclaration, fieldsToGenerate);
                var fileName = $"{classDeclaration.Identifier.Text}.Generated.cs";
                if (sourceCode.Length > 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "GEN0001",
                            title: "Code generator message",
                            messageFormat: "Generated source code {0} for MonoBehaviour {1}",
                            category: "Generation",
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None,
                        fileName, classDeclaration.Identifier.Text));
                    context.AddSource(fileName, sourceCode);
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "GEN0001",
                            title: "Code generator message",
                            messageFormat: "No source code to generate for MonoBehaviour {1}",
                            category: "Generation",
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None,
                        classDeclaration.Identifier.Text));
                }
            }
        }
    }
}