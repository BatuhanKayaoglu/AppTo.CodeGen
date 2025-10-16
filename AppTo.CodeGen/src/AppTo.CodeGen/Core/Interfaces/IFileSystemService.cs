namespace AppTo.CodeGen.Core.Interfaces;

/// <summary>
/// Service for file system operations
/// </summary>
public interface IFileSystemService
{
    void EnsureDirectory(string path);
    void WriteFile(string path, string content);
    void UpdateFile(string path, string content);
    bool FileExists(string path);
    string ReadFile(string path);
}
