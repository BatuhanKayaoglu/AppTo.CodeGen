using System;
using System.IO;
using System.Linq;

namespace AppTo.CodeGen.Commands.Templates;

public static class CommandGenerator
{
    public static string CreateCommand(string namespaceName, string featureName, string type = "command")
    {
        // Namespace'den project name'i çıkar
        var parts = namespaceName.Split('.');
        var projectName = string.Join(".", parts.Take(parts.Length - 2)); // Metropol.YODA.Application
        var responseNamespace = $"{projectName.Replace("Application", "Abstraction")}.{featureName}.Response";

        return $@"using Metropol.YODA.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {namespaceName};

public class {featureName}Command : MetropolCommand<{featureName}Response>
{{

}}
";
    }
}