using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FluentBuilderSourceGenerator;

[Generator]
public class FluentBuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("FluentBuilderAttribute.g.cs",
                SourceText.From(FluentBuilderMarker.Attribute, Encoding.UTF8));
        });

        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: (s, _) => IsClassWithAttribute(s),
                    transform: (ctx, _) => GetClassWithBuilderAttribute(ctx))
                .Where(x => x is not null)!;

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private bool IsClassWithAttribute(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private ClassDeclarationSyntax? GetClassWithBuilderAttribute(GeneratorSyntaxContext ctx)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "FluentBuilderAttribute")
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes,
        SourceProductionContext spc)
    {
        var classDefinitions = GetPropertyDefinitions(classes, compilation).ToList();

        foreach (var classDefinition in classDefinitions)
        {
            var source = SourceGeneratorHelper.GenerateBuilderClass(classDefinition);
            spc.AddSource($"{classDefinition.ClassName}Builder.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static IEnumerable<ClassDefinition> GetPropertyDefinitions(ImmutableArray<ClassDeclarationSyntax> classes,
        Compilation compilation)
    {
        foreach (var classDeclarationSyntax in classes)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var members = classSymbol.GetMembers();

            var properties = members.Where(x => x is IPropertySymbol)
                .Cast<IPropertySymbol>()
                .Select(x => new PropertyDefinition(x.Name, x.Type.Name))
                .ToList();

            yield return new ClassDefinition(classSymbol.Name, classSymbol.ContainingNamespace.ToString(), properties);
        }
    }
}