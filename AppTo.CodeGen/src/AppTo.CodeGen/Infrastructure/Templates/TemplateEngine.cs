using System.Collections.Generic;
using System.Linq;
using AppTo.CodeGen.Core.Enums;
using AppTo.CodeGen.Core.Interfaces;
using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Infrastructure.Templates;

/// <summary>
/// Template engine for generating code
/// </summary>
public class TemplateEngine : ITemplateEngine
{
    public string GenerateCommand(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");
        var responseNamespace = $"{baseProjectName}.Abstraction.{request.FeatureName}.Response";


        return $@"using {baseProjectName}.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {projectStructure.ProjectName}.{request.FeatureName}.Commands;

public class {request.FeatureName}Command : MetropolCommand<{request.FeatureName}Response>
{{

}}
";
    }

    public string GenerateCommandHandler(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");
        var responseNamespace = $"{baseProjectName}.Abstraction.{request.FeatureName}.Response";

        return $@"using {baseProjectName}.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {projectStructure.ProjectName}.{request.FeatureName}.Commands;

public class {request.FeatureName}CommandHandler : MetropolCommandHandler<{request.FeatureName}Command, {request.FeatureName}Response>
{{
    public override async Task<{request.FeatureName}Response?> Handle({request.FeatureName}Command request, CancellationToken cancellationToken)
    {{
        // TODO: Add your business logic here.
        return new {request.FeatureName}Response();
    }}
}}
";
    }

    public string GenerateCommandValidator(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");

        return $@"using {baseProjectName}.Infrastructure.Validation.Concrete;

namespace {projectStructure.ProjectName}.{request.FeatureName}.Commands;

public class {request.FeatureName}CommandValidator : MetropolValidator<{request.FeatureName}Command>
{{
    public {request.FeatureName}CommandValidator()
    {{
        
    }}
}}
";
    }

    public string GenerateQuery(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");
        var responseNamespace = $"{baseProjectName}.Abstraction.{request.FeatureName}.Response";

        return $@"using {baseProjectName}.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {projectStructure.ProjectName}.{request.FeatureName}.Queries;

public class {request.FeatureName}Query : MetropolQuery<{request.FeatureName}Response>
{{

}}  
";
    }

    public string GenerateQueryHandler(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");
        var responseNamespace = $"{baseProjectName}.Abstraction.{request.FeatureName}.Response";

        return $@"using {baseProjectName}.Infrastructure.CQRS.Concrete;
using {responseNamespace};

namespace {projectStructure.ProjectName}.{request.FeatureName}.Queries;

public class {request.FeatureName}QueryHandler : MetropolQueryHandler<{request.FeatureName}Query, {request.FeatureName}Response>
{{
    public override async Task<{request.FeatureName}Response?> Handle({request.FeatureName}Query request, CancellationToken cancellationToken)
    {{
        // TODO: Add your business logic here.
        return new {request.FeatureName}Response();
    }}
}}
";
    }

    public string GenerateQueryValidator(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var finalProjectName = request.ProjectName ?? projectStructure.ProjectName;
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");

        return $@"using {baseProjectName}.Infrastructure.Validation.Concrete;

namespace {projectStructure.ProjectName}.{request.FeatureName}.Queries;

public class {request.FeatureName}QueryValidator : MetropolValidator<{request.FeatureName}Query>
{{
    public {request.FeatureName}QueryValidator()
    {{
        
    }}
}}
";
    }

    public string GenerateRequest(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var propertiesCode = GeneratePropertiesCode(request.RequestProperties);
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");

        return $@"namespace {baseProjectName}.Abstraction.{request.FeatureName}.Request;

public class {request.FeatureName}Request
{{{propertiesCode}}}
";
    }

    public string GenerateResponse(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var propertiesCode = GeneratePropertiesCode(request.ResponseProperties);
        var baseProjectName = projectStructure.ProjectName.Replace(".Application", "");

        return $@"namespace {baseProjectName}.Abstraction.{request.FeatureName}.Response;

public class {request.FeatureName}Response
{{{propertiesCode}}}
";
    }

    public string GenerateEndpoint(FeatureGenerationRequest request, ProjectStructure projectStructure)
    {
        var httpMethod = request.Type == FeatureType.Command ? HttpMethodType.HttpPost : HttpMethodType.HttpGet;
        var routeName = System.Text.RegularExpressions.Regex.Replace(request.FeatureName, "(?<!^)([A-Z])", "-$1").ToLower();
        var parameterType = request.Type == FeatureType.Command ? ParameterType.FromBody : ParameterType.FromQuery;

        return $@"
    [{httpMethod}]
    [Route(""{routeName}"")]
    [ProducesResponseType(typeof(MetropolApiResponse<{request.FeatureName}Response>), (int)System.Net.HttpStatusCode.OK)]
    public async Task<MetropolApiResponse<{request.FeatureName}Response>> {request.FeatureName}(
        [{parameterType}] {request.FeatureName}Request request,
        CancellationToken cancellationToken)
    {{
        var response = await _cqrsProcessor.ProcessAsync(new {request.FeatureName}{request.Type}(), cancellationToken);
        return SetResponse(response);
    }}
";
    }

    private string GeneratePropertiesCode(List<PropertyDefinition> properties)
    {
        if (properties == null || !properties.Any())
            return string.Empty;

        var propertiesCode = "\n";
        foreach (var prop in properties)
        {
            propertiesCode += $"    public {prop.Type} {prop.Name} {{ get; set; }}\n\n";
        }

        return propertiesCode;
    }
}
