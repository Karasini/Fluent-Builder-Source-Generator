namespace FluentBuilderSourceGenerator;

internal class PropertyDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    
    public PropertyDefinition(string name, string typeName)
    {
        Name = name;
        Type = typeName;
    }
}
