namespace FluentBuilderSourceGenerator;

internal class ClassDefinition
{
    public string ClassName { get; set; }
    public string Namespace { get; set; }
    public List<PropertyDefinition> Properties { get; set; }

    public ClassDefinition(string className, string ns, List<PropertyDefinition> properties)
    {
        ClassName = className;
        Properties = properties;
        Namespace = ns;
    }
}
