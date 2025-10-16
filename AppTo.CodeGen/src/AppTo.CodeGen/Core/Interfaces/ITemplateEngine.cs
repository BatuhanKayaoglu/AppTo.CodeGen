using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Core.Interfaces;

/// <summary>
/// Template engine for generating code
/// </summary>
public interface ITemplateEngine
{
    string GenerateCommand(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateCommandHandler(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateCommandValidator(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateQuery(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateQueryHandler(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateQueryValidator(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateRequest(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateResponse(FeatureGenerationRequest request, ProjectStructure projectStructure);
    string GenerateEndpoint(FeatureGenerationRequest request, ProjectStructure projectStructure);
}
