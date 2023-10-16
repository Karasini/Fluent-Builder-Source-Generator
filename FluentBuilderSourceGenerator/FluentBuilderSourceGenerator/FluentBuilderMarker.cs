namespace FluentBuilderSourceGenerator;

internal static class FluentBuilderMarker
{
    public const string Attribute = @"
namespace FluentBuilderNamespace
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class FluentBuilderAttribute : System.Attribute
    {
    }
}";
}