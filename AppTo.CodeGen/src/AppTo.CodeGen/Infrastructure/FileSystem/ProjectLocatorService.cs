using AppTo.CodeGen.Core.Interfaces;
using AppTo.CodeGen.Core.Models;

namespace AppTo.CodeGen.Infrastructure.FileSystem;

/// <summary>
/// Service for locating project structure
/// </summary>
public class ProjectLocatorService : IProjectLocatorService
{
    public ProjectStructure LocateProjectStructure()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var srcPath = Directory.GetDirectories(currentDir, "src", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (srcPath == null)
            throw new DirectoryNotFoundException("❌ 'src' directory not found.");

        var applicationLayer = Directory.GetDirectories(srcPath, "*Application*", SearchOption.AllDirectories)
            .FirstOrDefault() ?? throw new DirectoryNotFoundException("❌ No folder containing 'Application' found inside 'src'.");

        var abstractionLayer = Directory.GetDirectories(srcPath, "*Abstraction*", SearchOption.AllDirectories)
            .FirstOrDefault() ?? throw new DirectoryNotFoundException("❌ No folder containing 'Abstraction' found inside 'src'.");

        var controllersLayer = Directory.GetDirectories(srcPath, "*Controllers*", SearchOption.AllDirectories)
            .FirstOrDefault() ?? throw new DirectoryNotFoundException("❌ No folder containing 'Controllers' found inside 'src'.");

        // Use Application layer name as the base project name
        var projectName = new DirectoryInfo(applicationLayer).Name;

        return new ProjectStructure
        {
            ApplicationLayer = applicationLayer,
            AbstractionLayer = abstractionLayer,
            ControllersLayer = controllersLayer,
            ProjectName = projectName // This will be TestProject.Application, which is correct for namespace
        };
    }
}
