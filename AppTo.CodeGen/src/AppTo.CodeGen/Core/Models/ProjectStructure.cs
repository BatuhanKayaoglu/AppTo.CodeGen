namespace AppTo.CodeGen.Core.Models;

/// <summary>
/// Represents the project structure information
/// </summary>
public record ProjectStructure
{
    public string ApplicationLayer { get; init; } = string.Empty;
    public string AbstractionLayer { get; init; } = string.Empty;
    public string ControllersLayer { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
}
