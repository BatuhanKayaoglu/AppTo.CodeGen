using System;
using System.IO;
using System.Linq;

namespace AppTo.CodeGen.Commands.Templates;

public static class CommandHandlerGenerator
{
    public static string CreateCommandHandler(string namespaceName, string featureName, string type = "command")
    {
        // Namespace'den project name'i çıkar
        var parts = namespaceName.Split('.');
        var projectName = string.Join(".", parts.Take(parts.Length - 2)); // Metropol.YODA.Application
        var responseNamespace = $"{projectName.Replace("Application", "Abstraction")}.{featureName}.Response";

        // Kod içeriği
        return $@"using Metropol.YODA.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {namespaceName};

public class {featureName}CommandHandler : MetropolCommandHandler<{featureName}Command, {featureName}Response>
{{
    public override async Task<{featureName}Response> Handle({featureName}Command request, CancellationToken cancellationToken)
    {{
        // TODO: İş mantığını burada uygula...
        return new {featureName}Response();
    }}
}}
";
    }
}