namespace AppTo.CodeGen.Commands.Templates;

public static class ResponseGenerator
{
    public static string CreateResponse(string namespaceName, string featureName)
    {
        return $@"namespace {namespaceName};

public class {featureName}Response
{{

}}
";
    }
}
