using System.Linq;

namespace AppTo.CodeGen.Commands.Templates;

public static class EndpointGenerator
{
    public static string CreateEndpoint(string namespaceName, string featureName, string controllerName, string type = "command")
    {
        var httpMethod = type.ToLower() == "command" ? "HttpPost" : "HttpGet";
        var routeName = featureName.ToLower();

        // Namespace'den project name'i çıkar
        var parts = namespaceName.Split('.');
        var projectName = string.Join(".", parts.Take(parts.Length - 2)); // Metropol.YODA
        //var requestNamespace = $"{projectName}.Abstraction.{featureName}.Request";
        //var responseNamespace = $"{projectName}.Abstraction.{featureName}.Response";
        //var commandNamespace = $"{projectName}.Application.{featureName}.Commands";

        return $@"


[{httpMethod}]
[Route(""{routeName}"")]
[ProducesResponseType(typeof(MetropolApiResponse<{featureName}Response>), (int)System.Net.HttpStatusCode.OK)]
public async Task<MetropolApiResponse<{featureName}Response>> {featureName}(
    [FromBody] {featureName}Request request,
    CancellationToken cancellationToken)
{{
    var response = await _cqrsProcessor.ProcessAsync(new {featureName}Command(), cancellationToken);
    return SetResponse(response);
}}
";
    }
}
