using System.Collections.Generic;
using AppTo.CodeGen.Core.Enums;

namespace AppTo.CodeGen.Core.Models;

/// <summary>
/// Represents a request to generate a feature
/// </summary>
public record FeatureGenerationRequest
{
    public string FeatureName { get; init; } = string.Empty;
    public FeatureType Type { get; init; } = FeatureType.Command;
    public string? Endpoint { get; init; }
    public string? ProjectName { get; init; }
    public string? Module { get; init; }
    public List<PropertyDefinition> RequestProperties { get; init; } = new();
    public List<PropertyDefinition> ResponseProperties { get; init; } = new();
    public bool GenerateValidator { get; init; } = true;
}
