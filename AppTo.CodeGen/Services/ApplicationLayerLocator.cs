using System.IO;
using System.Linq;

namespace AppTo.CodeGen.Services;

public interface IApplicationLayerLocator
{
    string LocateApplicationLayer();
    string LocateAbstractionLayer();
    string LocateControllersLayer();
}

public class ApplicationLayerLocator : IApplicationLayerLocator
{
    public string LocateApplicationLayer()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var srcPath = Directory.GetDirectories(currentDir, "src", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (srcPath == null)
            throw new DirectoryNotFoundException("❌ 'src' directory not found.");

        var applicationLayer = Directory.GetDirectories(srcPath, "*Application*", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (applicationLayer == null)
            throw new DirectoryNotFoundException("❌ No folder containing 'Application' found inside 'src'.");

        return applicationLayer;
    }

    public string LocateAbstractionLayer()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var srcPath = Directory.GetDirectories(currentDir, "src", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (srcPath == null)
            throw new DirectoryNotFoundException("❌ 'src' directory not found.");

        var abstractionLayer = Directory.GetDirectories(srcPath, "*Abstraction*", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (abstractionLayer == null)
            throw new DirectoryNotFoundException("❌ No folder containing 'Abstraction' found inside 'src'.");

        return abstractionLayer;
    }

    public string LocateControllersLayer()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var srcPath = Directory.GetDirectories(currentDir, "src", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (srcPath == null)
            throw new DirectoryNotFoundException("❌ 'src' directory not found.");

        var controllersLayer = Directory.GetDirectories(srcPath, "*Controllers*", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (controllersLayer == null)
            throw new DirectoryNotFoundException("❌ No folder containing 'Controllers' found inside 'src'.");

        return controllersLayer;
    }
}
