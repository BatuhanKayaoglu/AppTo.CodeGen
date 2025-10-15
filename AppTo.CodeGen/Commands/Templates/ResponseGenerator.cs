using System.Collections.Generic;
using System.Linq;

namespace AppTo.CodeGen.Commands.Templates;

public static class ResponseGenerator
{
    public static string CreateResponse(string namespaceName, string featureName, List<ResponseProperty> properties = null)
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

public class {featureName}Response
{{{propertiesCode}}}
";
    }
}

public class ResponseProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
}
