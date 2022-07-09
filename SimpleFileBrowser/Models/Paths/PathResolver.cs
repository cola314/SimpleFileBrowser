using System;
using System.IO;
using SimpleFileBrowser.Models.ApplicationEnvironments;

namespace SimpleFileBrowser.Models.Paths;

public class PathResolver
{
    private readonly ApplicationEnvironment _applicationEnvironment;

    public PathResolver(ApplicationEnvironment applicationEnvironment)
    {
        _applicationEnvironment = applicationEnvironment;
    }

    public string Resolve(string path)
    {
        string storagePath = NormalizePath(_applicationEnvironment.StoragePath);

        path = NormalizePath(path);
        path = Path.Combine(storagePath, path);

        if (!IsSubDirectory(storagePath, path))
        {
            throw new InvalidPathException();
        }

        return path;
    }

    private bool IsSubDirectory(string parentDirectory, string subDirectory)
    {
        parentDirectory = Path.GetFullPath(parentDirectory);
        subDirectory = Path.GetFullPath(subDirectory);

        var parentDirectories = parentDirectory.Split(Path.DirectorySeparatorChar);
        var subDirectories = subDirectory.Split(Path.DirectorySeparatorChar);

        if (parentDirectories.Length >= subDirectories.Length)
            return false;

        for (int i = 0; i < parentDirectories.Length; i++)
        {
            if (parentDirectories[i] != subDirectories[i])
                return false;
        }

        return true;
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', Path.DirectorySeparatorChar);
    }
}