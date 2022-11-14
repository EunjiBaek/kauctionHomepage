using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KA.Web.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost]
        [Route("/api/File/Upload/{Type}")]
        public async Task<IActionResult> Upload(string type = "")
        {
            var files = Request.Form.Files;
            var uploadFileCount = 0;
            var fileNames = new List<string>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    try
                    {
                        var fileName = formFile.FileName;

                        using var stream = System.IO.File.Create(FileHelper.GetFolderPath(type, Config.ContentPath) + "\\" + fileName);
                        await formFile.CopyToAsync(stream);

                        uploadFileCount++;
                        fileNames.Add(fileName);
                    }
                    catch (Exception) { }
                }
            }

            return Ok(new FileUploadResult()
            {
                FileCount = files.Count,
                UploadFileCount = uploadFileCount,
                FileNames = fileNames.ToArray()
            });
        }
    }
}
