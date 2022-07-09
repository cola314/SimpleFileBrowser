using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleFileBrowser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleFileBrowser.Services;

namespace SimpleFileBrowser.Controllers;

[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly FileService _fileService;

    public FileController(ILogger<FileController> logger, FileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    [HttpGet("files/{path}")]
    public ActionResult<FileInformation[]> GetFiles(string path)
    {
        path = NormalizePath(path);

        try
        {
            return _fileService.GetFiles(path);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', Path.DirectorySeparatorChar);
    }

    [HttpGet("folders/{path}")]
    public ActionResult<FolderInformation[]> GetFolders(string path)
    {
        path = NormalizePath(path);
        try
        {
            return _fileService.GetFolders(path);
        }
        catch (DirectoryNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("hash/{path}")]
    public ActionResult<HashInformation> GetHash(string path)
    {
        path = NormalizePath(path);

        try
        {
            return _fileService.GetHash(path);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("rename/{path}/{name}")]
    public ActionResult RenameFileFolder(string path, string name)
    {
        path = NormalizePath(path);

        string newPath = Path.Join(Path.GetDirectoryName(path), Path.GetFileName(name));
        try
        {
            _fileService.Rename(path, newPath);
            return Ok();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("folder/{path}/{name}")]
    public ActionResult CreateFolder(string path, string name)
    {
        path = NormalizePath(path);

        string newPath = Path.Join(path, Path.GetFileName(name));

        try
        {
            _fileService.CreateFolder(path, newPath);
            return Ok();
        }
        catch (DirectoryNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("delete/{path}")]
    public ActionResult DeleteFileFolder(string path)
    {
        path = NormalizePath(path);

        try
        {
            _fileService.DeleteFileFolder(path);
            return Ok();
        }
        catch (DirectoryNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("downloadFile/{path}")]
    public FileResult DownloadFile(string path)
    {
        path = NormalizePath(path);

        return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
    }

    [HttpGet("downloadFolder/{path}")]
    public FileResult DownloadFolder(string path)
    {
        path = NormalizePath(path);

        if (!Directory.Exists("temp"))
        {
            Directory.CreateDirectory("temp");
        }

        foreach (var file in Directory.GetFiles("temp"))
        {
            if (DateTime.Now - System.IO.File.GetCreationTime(file) > TimeSpan.FromMinutes(10))
            {
                _logger.LogInformation(file + " delete");

                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, file + " delete fail");
                }
            }
        }

        string tempFile = Path.Combine("temp", $"{DateTime.Now.Ticks.ToString()}");

        ZipFile.CreateFromDirectory(path, tempFile);

        return File(System.IO.File.OpenRead(tempFile), "application/octet-stream", Path.GetFileName(path) + ".zip");
    }

    [HttpPost("upload/{path}")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public ActionResult UploadFileAsync(string path)
    {
        path = NormalizePath(path);

        try
        {
            var files = Request.Form.Files;

            foreach (IFormFile file in files)
            {
                if (file.Length == 0)
                    continue;

                var filePath = Path.Join(path, Path.GetFileName(file.FileName));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return new OkObjectResult("Yes");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }
}