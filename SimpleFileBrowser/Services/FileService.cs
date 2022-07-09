using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SimpleFileBrowser.Models;
using SimpleFileBrowser.Models.Paths;

namespace SimpleFileBrowser.Services;

public class FileService
{
    private readonly PathResolver _pathResolver;

    public FileService(PathResolver pathResolver)
    {
        _pathResolver = pathResolver;
    }

    public FileInformation[] GetFiles(string path)
    {
        path = _pathResolver.Resolve(path);

        return Directory.GetFiles(path)
            .Select(filePath => new FileInfo(filePath))
            .Select(fileInfo => new FileInformation()
            {
                Name = fileInfo.Name,
                FullName = fileInfo.FullName.Replace("/", "\\"),
                Size = fileInfo.Length,
                LastModifiedDate = fileInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh-mm-ss")
            })
            .ToArray();
    }

    public FolderInformation[] GetFolders(string path)
    {
        path = _pathResolver.Resolve(path);

        return Directory.GetDirectories(path)
            .Select(folderPath => new DirectoryInfo(folderPath))
            .Select(folderInfo => new FolderInformation()
            {
                Name = folderInfo.Name,
                FullName = folderInfo.FullName.Replace("/", "\\"),
                LastModifiedDate = folderInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh-mm-ss")
            })
            .ToArray();
    }

    public HashInformation GetHash(string path)
    {
        path = _pathResolver.Resolve(path);
        var file = new FileInfo(path);

        return new HashInformation()
        {
            Md5 = BitConverter.ToString(MD5.Create().ComputeHash(file.OpenRead())).Replace("-", ""),
            Sha1 = BitConverter.ToString(SHA1.Create().ComputeHash(file.OpenRead())).Replace("-", ""),
            Sha256 = BitConverter.ToString(SHA256.Create().ComputeHash(file.OpenRead())).Replace("-", "")
        };
    }

    public void Rename(string path, string newPath)
    {
        path = _pathResolver.Resolve(path);

        if (Directory.Exists(path))
        {
            Directory.Move(path, newPath);
        }
        else if (File.Exists(path))
        {
            File.Move(path, newPath);
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public void CreateFolder(string path, string name)
    {
        path = _pathResolver.Resolve(path);
        string folder = Path.Join(path, Path.GetFileName(name));
        Directory.CreateDirectory(folder);
    }

    public void DeleteFileFolder(string path)
    {
        path = _pathResolver.Resolve(path);

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            throw new FileNotFoundException();
        }
    }
}