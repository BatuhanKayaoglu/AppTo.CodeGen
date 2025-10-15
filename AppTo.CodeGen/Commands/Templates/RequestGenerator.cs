using System.Collections.Generic;
using System.Linq;

namespace AppTo.CodeGen.Commands.Templates;

public static class RequestGenerator
{
    public static string CreateRequest(string namespaceName, string featureName, List<RequestProperty> properties = null)
    {
        var propertiesCode = "";

        if (properties != null && properties.Any())
        {
            propertiesCode = "\n";
            foreach (var prop in properties)
            {
                propertiesCode += $"    public {prop.Type} {prop.Name} {{ get; set; }}\n\n";
            }
        }

        return $@"namespace {namespaceName};

public class {featureName}Request
{{{propertiesCode}}}
";
    }
}

public class RequestProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
}
