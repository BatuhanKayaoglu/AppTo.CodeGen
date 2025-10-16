using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Core.Interfaces;

/// <summary>
/// Service for locating project structure
/// </summary>
public interface IProjectLocatorService
{
    ProjectStructure LocateProjectStructure();
}
