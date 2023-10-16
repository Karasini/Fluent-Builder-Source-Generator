using System.Text;

namespace FluentBuilderSourceGenerator;

internal static class SourceGeneratorHelper
{
    public static string GenerateBuilderClass(ClassDefinition classDefinition)
    {
        var sb = new StringBuilder();

        sb.Append(@$"
using {classDefinition.Namespace};

internal class {classDefinition.ClassName}Builder
{{
    {GetPrivateFields(classDefinition.Properties)}
    public {classDefinition.ClassName} Build()
    {{
        return new {classDefinition.ClassName}
        {{
            {GetPropertiesAssignments(classDefinition.Properties)}
        }};
    }}

    {GetBuilderMethodDefinitions(classDefinition.ClassName, classDefinition.Properties)}

    }}
");

        return sb.ToString();
    }

    private static string GetPrivateFields(List<PropertyDefinition> classDefinitionProperties)
    {
        var sb = new StringBuilder();
        foreach (var propertyDefinition in classDefinitionProperties)
        {
            sb.Append($"private {propertyDefinition.Type} _{propertyDefinition.Name};");
        }

        return sb.ToString();
    }

    private static string GetPropertiesAssignments(List<PropertyDefinition> classDefinitionProperties)
    {
        var sb = new StringBuilder();

        foreach (var propertyDefinition in classDefinitionProperties)
        {
            sb.Append($"{propertyDefinition.Name} = _{propertyDefinition.Name},");
        }

        return sb.ToString();
    }

    private static string GetBuilderMethodDefinitions(string className,
        List<PropertyDefinition> classDefinitionProperties)
    {
        var sb = new StringBuilder();

        foreach (var propertyDefinition in classDefinitionProperties)
        {
            sb.Append($@"
    public {className}Builder With{propertyDefinition.Name}(string value)
    {{
        _{propertyDefinition.Name} = value;

        return this;
    }}
");
        }

        return sb.ToString();
    }
}