using System.Linq;
using AppTo.CodeGen.Services;

namespace AppTo.CodeGen.Commands.Templates;

public static class QueryValidatorGenerator
{
    public static string CreateQueryValidator(string namespaceName, string featureName, string type = "query", string projectName = null)
    {
        // Proje adını al (parametre verilmişse onu kullan, yoksa otomatik algıla)
        var finalProjectName = projectName ?? new ProjectNameService().GetProjectName();

        return $@"using {finalProjectName}.Infrastructure.Validation.Concrete;

namespace {namespaceName};

public class {featureName}QueryValidator : MetropolValidator<{featureName}Query>
{{
    public {featureName}QueryValidator()
    {{
        
    }}
}}
";
    }
}
