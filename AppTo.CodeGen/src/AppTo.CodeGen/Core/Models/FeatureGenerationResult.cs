using System.Collections.Generic;

namespace AppTo.CodeGen.Core.Models;

/// <summary>
/// Result of feature generation operation
/// </summary>
public record FeatureGenerationResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> GeneratedFiles { get; init; } = new();
    public List<string> Messages { get; init; } = new();
}
