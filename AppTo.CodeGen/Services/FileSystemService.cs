using System;
using System.IO;

// Amaç: Dosya sistemi işlemlerini yönetmek (klasör oluşturma, dosya yazma vb.)
// Amaç: Dosya ve klasör oluşturma işlerini tek sorumlulukla yapmak (SRP).
namespace AppTo.CodeGen.Services;

public interface IFileSystemService
{
    void EnsureDirectory(string path);
    void WriteFile(string path, string content);
}

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
}
