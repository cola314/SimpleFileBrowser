using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleFileBrowser.Models;
using System;
using System.IO;
using System.IO.Compression;
using SimpleFileBrowser.Filters;
using SimpleFileBrowser.Models.Paths;
using SimpleFileBrowser.Services;

namespace SimpleFileBrowser.Controllers;

[FileExceptionFilter]
[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly FileService _fileService;
    private readonly PathResolver _pathResolver;

    public FileController(ILogger<FileController> logger, FileService fileService, PathResolver pathResolver)
    {
        _logger = logger;
        _fileService = fileService;
        _pathResolver = pathResolver;
    }

    [HttpGet("files/{path}")]
    public ActionResult<FileInformation[]> GetFiles(string path)
    {
        return _fileService.GetFiles(path);
    }

    [HttpGet("folders/{path}")]
    public ActionResult<FolderInformation[]> GetFolders(string path)
    {
        return _fileService.GetFolders(path);
    }

    [HttpGet("hash/{path}")]
    public ActionResult<HashInformation> GetHash(string path)
    {
        return _fileService.GetHash(path);
    }

    [HttpPost("rename/{path}/{name}")]
    public ActionResult RenameFileFolder(string path, string name)
    {
        string newPath = Path.Join(Path.GetDirectoryName(path), Path.GetFileName(name));

        _fileService.Rename(path, newPath);
        return Ok();
    }

    [HttpPost("folder/{path}/{name}")]
    public ActionResult CreateFolder(string path, string name)
    {
        _fileService.CreateFolder(path, name);
        return Ok();
    }

    [HttpPost("delete/{path}")]
    public ActionResult DeleteFileFolder(string path)
    {
        _fileService.DeleteFileFolder(path);
        return Ok();
    }

    [HttpGet("downloadFile/{path}")]
    public FileResult DownloadFile(string path)
    {
        path = _pathResolver.Resolve(path);

        return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
    }

    [HttpGet("downloadFolder/{path}")]
    public FileResult DownloadFolder(string path)
    {
        path = _pathResolver.Resolve(path);

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
        path = _pathResolver.Resolve(path);

        try
        {
            var files = Request.Form.Files;

            foreach (IFormFile file in files)
            {
                if (file.Length == 0)
                    continue;

                var filePath = Path.Join(path, Path.GetFileName(file.FileName));

                using var fileStream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fileStream);
            }
            return new OkObjectResult("Yes");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }
}