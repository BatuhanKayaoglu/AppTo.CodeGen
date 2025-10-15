using System.Linq;
using AppTo.CodeGen.Services;

namespace AppTo.CodeGen.Commands.Templates;

public static class CommandValidatorGenerator
{
    public static string CreateCommandValidator(string namespaceName, string featureName, string type = "command", string projectName = null)
    {
        // Proje adını al (parametre verilmişse onu kullan, yoksa otomatik algıla)
        var finalProjectName = projectName ?? new ProjectNameService().GetProjectName();

        return $@"using {finalProjectName}.Infrastructure.Validation;

namespace {namespaceName};

public class {featureName}CommandValidator : MetropolValidator<{featureName}Command>
{{
    public {featureName}CommandValidator()
    {{
        
    }}
}}
";
    }
}
