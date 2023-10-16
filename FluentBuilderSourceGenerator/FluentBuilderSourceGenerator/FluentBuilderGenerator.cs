using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FluentBuilderSourceGenerator;

[Generator]
public class FluentBuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("FluentBuilderAttribute.g.cs", SourceText.From(FluentBuilderMarker.Attribute, Encoding.UTF8));
        });
    }
}