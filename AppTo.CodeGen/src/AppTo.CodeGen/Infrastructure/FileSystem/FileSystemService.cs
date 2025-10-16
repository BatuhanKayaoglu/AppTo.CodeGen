using AppTo.CodeGen.Core.Interfaces;

namespace AppTo.CodeGen.Infrastructure.FileSystem;

/// <summary>
/// Implementation of file system service
/// </summary>
public class FileSystemService : IFileSystemService
{
    public void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public void WriteFile(string path, string content)
    {
        if (File.Exists(path))
            throw new InvalidOperationException($"⚠️ File already exists: {path}");

        File.WriteAllText(path, content);
    }

    public void UpdateFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string ReadFile(string path)
    {
        return File.ReadAllText(path);
    }
}
