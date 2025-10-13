using System.Linq;
using AppTo.CodeGen.Services;

namespace AppTo.CodeGen.Commands.Templates;

public static class QueryGenerator
{
    public static string CreateQuery(string namespaceName, string featureName, string type = "query", string projectName = null)
    {
        // Proje adını al (parametre verilmişse onu kullan, yoksa otomatik algıla)
        var finalProjectName = projectName ?? new ProjectNameService().GetProjectName();

        // Namespace'den project name'i çıkar
        var parts = namespaceName.Split('.');
        var fullProjectName = string.Join(".", parts.Take(parts.Length - 2)); // ProjectName.Application
        var responseNamespace = $"{fullProjectName.Replace("Application", "Abstraction")}.{featureName}.Response";

        return $@"using {finalProjectName}.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {namespaceName};

public class {featureName}Query : MetropolQuery<{featureName}Response>
{{

}}  
";
    }
}