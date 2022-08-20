using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleFileBrowser.Models.Paths;

namespace SimpleFileBrowser.Filters;

public class FileExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        IActionResult result = context.Exception switch
        {
            FileNotFoundException or DirectoryNotFoundException => new NotFoundResult(),
            InvalidPathException => new BadRequestResult(),
            _ => null,
        };
        
        if (result != null)
            context.Result = result;
    }
}