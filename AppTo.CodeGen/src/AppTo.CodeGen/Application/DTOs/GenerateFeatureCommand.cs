using System.Collections.Generic;
using System.Linq;
using AppTo.CodeGen.Core.Enums;
using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Application.DTOs;

/// <summary>
/// Command for generating a feature
/// </summary>
public record GenerateFeatureCommand : FeatureGenerationRequest
{
    public GenerateFeatureCommand(
        string featureName,
        FeatureType type = FeatureType.Command,
        string? endpoint = null,
        string? projectName = null,
        string? module = null,
        string? requestProperties = null,
        string? responseProperties = null,
        bool generateValidator = true)
    {
        FeatureName = featureName;
        Type = type;
        Endpoint = endpoint;
        ProjectName = projectName;
        Module = module;
        GenerateValidator = generateValidator;

        RequestProperties = ParseProperties(requestProperties);
        ResponseProperties = ParseProperties(responseProperties);
    }

    private static List<PropertyDefinition> ParseProperties(string? properties)
    {
        if (string.IsNullOrEmpty(properties))
            return new List<PropertyDefinition>();

        return properties.Split(',')
            .Select(prop => prop.Trim().Split(':'))
            .Where(parts => parts.Length >= 2)
            .Select(parts => new PropertyDefinition
            {
                Name = parts[0].Trim(),
                Type = parts[1].Trim()
            })
            .ToList();
    }
}
