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
using System.Threading.Tasks;

namespace SimpleFileBrowser.Controllers
{
    [ApiController]
    [Route("api")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        [HttpGet("files/{path}")]
        public ActionResult<FileInformation[]> GetFiles(string path)
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path)
                    .Select(filePath => new FileInfo(filePath))
                    .Select(fileInfo => new FileInformation()
                    {
                        Name = fileInfo.Name,
                        FullName = fileInfo.FullName,
                        Size = fileInfo.Length,
                        LastModifiedDate = fileInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh-mm-ss")
                    })
                    .ToArray();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("folders/{path}")]
        public ActionResult<FolderInformation[]> GetFolders(string path)
        {
            if (Directory.Exists(path))
            {
                return Directory.GetDirectories(path)
                    .Select(folderPath => new DirectoryInfo(folderPath))
                    .Select(folderInfo => new FolderInformation()
                    {
                        Name = folderInfo.Name,
                        FullName = folderInfo.FullName,
                        LastModifiedDate = folderInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh-mm-ss")
                    })
                    .ToArray();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("hash/{path}")]
        public ActionResult<HashInformation> GetHash(string path, string method)
        {
            if (System.IO.File.Exists(path))
            {
                var file = new FileInfo(path);

                return new HashInformation()
                {
                    Md5 = BitConverter.ToString(MD5.Create().ComputeHash(file.OpenRead())).Replace("-", ""),
                    Sha1 = BitConverter.ToString(SHA1.Create().ComputeHash(file.OpenRead())).Replace("-", ""),
                    Sha256 = BitConverter.ToString(SHA256.Create().ComputeHash(file.OpenRead())).Replace("-", "")
                };
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("rename/{path}/{name}")]
        public ActionResult RenameFileFolder(string path, string name)
        {
            string newPath = Path.Join(Path.GetDirectoryName(path), Path.GetFileName(name));

            if (Directory.Exists(path))
            {
                Directory.Move(path, newPath);
                return Ok();
            }
            else if (System.IO.File.Exists(path))
            {
                System.IO.File.Move(path, newPath);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("folder/{path}/{name}")]
        public ActionResult CreateFolder(string path, string name)
        {
            string newPath = Path.Join(path, Path.GetFileName(name));

            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(newPath);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("delete/{path}")]
        public ActionResult DeleteFileFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return Ok();
            }
            else if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("downloadFile/{path}")]
        public FileResult DownloadFile(string path)
        {
            return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
        }

        [HttpGet("downloadFolder/{path}")]
        public FileResult DownloadFolder(string path)
        {
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
        [Consumes("multipart/form-data")]
        public ActionResult UploadFileAsync(string path)
        {
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
}
