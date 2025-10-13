using System.Linq;
using AppTo.CodeGen.Models;
using AppTo.CodeGen.Services;

namespace AppTo.CodeGen.Commands.Templates;

public static class EndpointGenerator
{
    public static string CreateEndpoint(string namespaceName, string featureName, string controllerName, FeatureType type = FeatureType.Command)
    {
        var httpMethod = type == FeatureType.Command ? HttpMethodType.HttpPost : HttpMethodType.HttpGet;
        // Convert camelCase to kebab-case (QrSaleTest -> qr-sale-test)
        var routeName = System.Text.RegularExpressions.Regex.Replace(featureName, "(?<!^)([A-Z])", "-$1").ToLower();

        // Proje adını dinamik olarak al
        var projectNameService = new ProjectNameService();
        var projectName = projectNameService.GetProjectName();

        // Namespace'den project name'i çıkar
        var parts = namespaceName.Split('.');
        var fullProjectName = string.Join(".", parts.Take(parts.Length - 2)); // ProjectName
        var requestNamespace = $"{fullProjectName}.Abstraction.{featureName}.Request";
        var responseNamespace = $"{fullProjectName}.Abstraction.{featureName}.Response";
        var commandNamespace = $"{fullProjectName}.Application.{featureName}.{(type == FeatureType.Command ? "Commands" : "Queries")}";

        var commandType = type == FeatureType.Command ? "Command" : "Query";
        var parameterType = type == FeatureType.Command ? ParameterType.FromBody : ParameterType.FromQuery;

        return $@"
    [{httpMethod}]
    [Route(""{routeName}"")]
    [ProducesResponseType(typeof(MetropolApiResponse<{featureName}Response>), (int)System.Net.HttpStatusCode.OK)]
    public async Task<MetropolApiResponse<{featureName}Response>> {featureName}(
        [{parameterType}] {featureName}Request request,
        CancellationToken cancellationToken)
    {{
        var response = await _cqrsProcessor.ProcessAsync(new {featureName}{commandType}(), cancellationToken);
        return SetResponse(response);
    }}
";
    }
}
