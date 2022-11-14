using ImageMagick;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KA.Web.Public.Controllers
{
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
            var description = string.Empty;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    try
                    {
                        var fileName = formFile.FileName;

                        // [#745/개선] iOS 생성 이미지 HEIC 파일의 웹 뷰잉 개선 - heic 확장자 체크하여 저장 처리 분리
                        if (formFile.FileName[formFile.FileName.LastIndexOf('.')..].ToLower().Contains(".heic"))
                        {
                            var newFileName = fileName.ToUpper().Replace(".HEIC", ".jpg");
                            using var image = new MagickImage(formFile.OpenReadStream());
                            image.Format = MagickFormat.Jpg;
                            image.Write(FileHelper.GetFolderPath(type, Config.ContentPath, LoginInfo.ID) + "\\" + newFileName);
                            fileNames.Add(newFileName);
                        }
                        else
                        {
                            using var stream = System.IO.File.Create(FileHelper.GetFolderPath(type, Config.ContentPath, LoginInfo.ID) + "\\" + fileName);
                            await formFile.CopyToAsync(stream);
                            fileNames.Add(fileName);
                        }

                        uploadFileCount++;
                    }
                    catch (Exception Ex) { description = Ex.ToString(); }
                }
            }

            return Ok(new FileUploadResult()
            {
                FileCount = files.Count,
                UploadFileCount = uploadFileCount,
                FileNames = fileNames.ToArray(),
                Description = description
            });
        }
    }
}
