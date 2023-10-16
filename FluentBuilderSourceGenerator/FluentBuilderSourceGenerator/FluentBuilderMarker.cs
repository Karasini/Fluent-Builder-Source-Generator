namespace FluentBuilderSourceGenerator;

internal static class FluentBuilderMarker
{
    public const string Attribute = @"

    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal class FluentBuilderAttribute : System.Attribute
    {
    }";
}