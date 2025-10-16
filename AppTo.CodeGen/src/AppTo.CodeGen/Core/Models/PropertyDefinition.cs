namespace AppTo.CodeGen.Core.Models;

/// <summary>
/// Represents a property definition for generated classes
/// </summary>
public record PropertyDefinition
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}
