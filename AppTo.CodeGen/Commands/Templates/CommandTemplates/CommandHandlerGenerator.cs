using System;
using System.IO;
using System.Linq;
using AppTo.CodeGen.Services;

namespace AppTo.CodeGen.Commands.Templates;

public static class CommandHandlerGenerator
{
    public static string CreateCommandHandler(string namespaceName, string featureName, string type = "command", string projectName = null)
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

public class {featureName}CommandHandler : MetropolCommandHandler<{featureName}Command, {featureName}Response>
{{
    public override async Task<{featureName}Response> Handle({featureName}Command request, CancellationToken cancellationToken)
    {{
        // TODO: Business logic
        return new {featureName}Response();
    }}
}}
";
    }
}