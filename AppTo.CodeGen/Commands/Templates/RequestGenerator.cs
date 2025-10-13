namespace AppTo.CodeGen.Commands.Templates;

public static class RequestGenerator
{
    public static string CreateRequest(string namespaceName, string featureName)
    {
        return $@"namespace {namespaceName};

public class {featureName}Request
{{
    // TODO: Request özelliklerini buraya ekleyin
}}
";
    }
}
