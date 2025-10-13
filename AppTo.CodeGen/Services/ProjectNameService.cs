using System;
using System.IO;
using System.Linq;

namespace AppTo.CodeGen.Services;

public interface IProjectNameService
{
    string GetProjectName();
}

public class ProjectNameService : IProjectNameService
{
    public string GetProjectName()
    {
        var currentDir = Directory.GetCurrentDirectory();

        // src klasörünü bul ve içindeki ilk projeyi al
        var srcPath = Directory.GetDirectories(currentDir, "src", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (srcPath == null)
            throw new DirectoryNotFoundException("❌ 'src' directory not found.");

        var projectDirs = Directory.GetDirectories(srcPath);
        if (projectDirs.Length == 0)
            throw new DirectoryNotFoundException("❌ No project directories found in 'src'.");

        // İlk proje klasörünün adını al
        var firstProjectDir = projectDirs[0];
        var dirName = new DirectoryInfo(firstProjectDir).Name;

        return dirName;
    }
}
