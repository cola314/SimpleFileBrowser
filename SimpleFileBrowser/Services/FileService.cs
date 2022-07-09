using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using SimpleFileBrowser.Controllers;
using SimpleFileBrowser.Models;

namespace SimpleFileBrowser.Services;

public class FileService
{
    private FileController _fileController;

    public FileService(FileController fileController)
    {
        _fileController = fileController;
    }

    public FileInformation[] GetFiles(string path)
    {
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

    public void CreateFolder(string path, string newPath)
    {
        Directory.CreateDirectory(newPath);
    }

    public void DeleteFileFolder(string path)
    {
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