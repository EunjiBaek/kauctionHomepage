using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using KA.Entities.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KA.Entities.Helpers
{
    public static class FileHelper
    {
        public static bool MakeFolder(string fileName, string rootPath)
        {
            try
            {
                var pathInfo = fileName.Split('\\');
                for (int i = 0; i < pathInfo.Length - 1; i++)
                {
                    if (string.IsNullOrWhiteSpace(pathInfo[i])) continue;

                    rootPath += "\\" + pathInfo[i];
                    if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool CopyTo(string orgFileName, string targetFileName)
        {
            try
            {
                using var fs = File.OpenRead(orgFileName);
                return CopyTo(fs, targetFileName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CopyTo(Stream file, string fileName)
        {
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CopyTo(Stream file, string fileName, string rootPath)
        {
            try
            {
                var pathInfo = fileName.Split('\\');
                for (int i = 0; i < pathInfo.Length - 1; i++)
                {
                    if (string.IsNullOrWhiteSpace(pathInfo[i])) continue;

                    rootPath += "\\" + pathInfo[i];
                    if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                }

                return CopyTo(file, rootPath + "\\" + Path.GetFileName(fileName));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static FileUploadResult CopyTo(FileUploadInfo fileUploadInfo)
        {
            var result = true;
            var savedFileNames = new List<string>();
            var fileIndex = 0;

            if (fileUploadInfo.ServerType.Equals("AWS"))
            {
                foreach (var item in fileUploadInfo.FileNames)
                {
                    var privateMode = item.ToLower().Contains("identification");
                    try
                    {
                        fileIndex++;
                        if (result)
                        {
                            using var client = new AmazonS3Client(
                                privateMode ? fileUploadInfo.AccessKeyPrivate : fileUploadInfo.AccessKey,
                                privateMode ? fileUploadInfo.SecretKeyPrivate : fileUploadInfo.SecretKey, 
                                Amazon.RegionEndpoint.APNortheast2);
                            var file = fileUploadInfo.FilePath + (item.StartsWith("\\") ? item : "\\" + item);
                            using var fs = File.OpenRead(file);

                            using TransferUtility utility = new TransferUtility(client);
                            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
                            {
                                BucketName = privateMode ? fileUploadInfo.BucketNameHomepatePrivate : fileUploadInfo.BucketNameHomepage,
                                Key = GetFileName(fileUploadInfo.Target, item, fileIndex, fileUploadInfo.Etc, fileUploadInfo.Etc2),
                                InputStream = fs
                            };
                            utility.Upload(request);

                            if (fileUploadInfo.IsKofficeCopy)
                            {
                                using var fs2 = File.OpenRead(file);
                                using TransferUtility utility2 = new TransferUtility(client);
                                TransferUtilityUploadRequest request2 = new TransferUtilityUploadRequest
                                {
                                    BucketName = privateMode ? fileUploadInfo.BucketNameKofficePrivate : fileUploadInfo.BucketNameKoffice,
                                    Key = GetFileNameKoffice(fileUploadInfo.Target, item, fileUploadInfo.Etc2),
                                    InputStream = fs2
                                };
                                utility2.Upload(request2);
                            }

                            if (fileUploadInfo.Target.Equals("Consign"))
                            {
                                savedFileNames.Add(request.Key[(request.Key.LastIndexOf('/') + 1)..]);
                            }
                            else
                            {
                                savedFileNames.Add(request.Key);
                            }
                        }
                    }
                    catch (Exception) { result = false; }
                }
            }
            else
            {
                foreach (var item in fileUploadInfo.FileNames)
                {
                    try
                    {
                        fileIndex++;

                        if (result)
                        {
                            var file = fileUploadInfo.FilePath + "\\" + item;
                            using var fs = File.OpenRead(file);
                            var savedFilePath = GetFileName(fileUploadInfo.Target, item, fileIndex, fileUploadInfo.Etc);
                            FileHelper.CopyTo(fs, savedFilePath);

                            if (fileUploadInfo.IsKofficeCopy)
                            {
                                FileHelper.CopyTo(fs, GetFileNameKoffice(fileUploadInfo.Target, item, fileUploadInfo.Etc2));
                            }
                            fs.Dispose();

                            if (fileUploadInfo.Target.Equals("Consign"))
                            {
                                savedFileNames.Add(savedFilePath[(savedFilePath.LastIndexOf('/') + 1)..]);
                            }
                            else
                            {
                                savedFileNames.Add(savedFilePath);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        var file = fileUploadInfo.FilePath + "\\" + item;

                        throw;
                    }
                }
            }
            return new FileUploadResult { Result = result, FileNames = savedFileNames };
        }

        public static async Task MoveTo(FileUploadInfo fileUploadInfo)
        {
            var fileIndex = 0;

            foreach (var item in fileUploadInfo.FileNames)
            {
                var privateMode = item.ToLower().Contains("identification");
                try
                {
                    fileIndex++;
                    using var client = new AmazonS3Client(
                        privateMode ? fileUploadInfo.AccessKeyPrivate : fileUploadInfo.AccessKey, 
                        privateMode ? fileUploadInfo.SecretKeyPrivate : fileUploadInfo.SecretKey, 
                        Amazon.RegionEndpoint.APNortheast2);
                    CopyObjectRequest request = new CopyObjectRequest
                    {
                        SourceBucket = privateMode ? fileUploadInfo.BucketNameHomepatePrivate : fileUploadInfo.BucketNameHomepage,
                        SourceKey = GetFileName(fileUploadInfo.Target, item, fileIndex, fileUploadInfo.Etc),
                        DestinationBucket = privateMode ? fileUploadInfo.BucketNameHomepatePrivate : fileUploadInfo.BucketNameHomepage,
                        DestinationKey = GetFileName(fileUploadInfo.Target, item, fileIndex, fileUploadInfo.Etc2)
                    };
                    CopyObjectResponse response = await client.CopyObjectAsync(request);
                    var requestResult = new DeleteObjectRequest
                    {
                        BucketName = fileUploadInfo.BucketNameHomepage,
                        Key = GetFileName(fileUploadInfo.Target, item, fileIndex, fileUploadInfo.Etc)
                    };
                    var responseResult = await client.DeleteObjectAsync(requestResult);
                }
                catch (Exception) { }
            }
        }

        public static string GetFolderPath(string type, string rootPath = "", string etc = "")
        {
            var filePath = rootPath + "\\" + type;
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            if (type.ToLower().Equals("consign")) // 위탁신청 (ex. /Consign/2020/12/01)
            {
                filePath += $"\\{DateTime.Now:yyyy}";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += $"\\{DateTime.Now:MM}";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += $"\\{DateTime.Now:dd}";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            }
            else if (type.ToLower().Equals("identification") || type.ToLower().Equals("companyregdoc") || type.ToLower().Equals("companybusinesscard"))
            {
                filePath = rootPath + "\\Temp";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += "\\Member";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += "\\Join";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += "\\" + etc;
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                switch (type.ToLower())
                {
                    case "identification": filePath += "\\identification"; break;
                    case "companyregdoc": filePath += "\\memBusinessLicenseDoc"; break;
                    case "companybusinesscard": filePath += "\\memBusinessCard"; break;
                    default: break;
                }
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            }
            else if (type.ToLower().Equals("imgfileupload"))
            {
                filePath = rootPath + "\\" + type;
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                filePath += $"\\{DateTime.Now:yyyy_MM}";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            }

            return filePath;
        }

        public static string GetFolderPath(string type, ref string fileName)
        {
            return GetFolderPath(type) + (string.IsNullOrWhiteSpace(fileName) ? "" : "\\" + GetFileName(type, fileName));
        }

        public static string GetFileName(string type, string fileName, int index = 0, string etc = "", string etc2 = "")
        {
            if (type.Equals("Consign"))
            {
                fileName = string.IsNullOrWhiteSpace(etc2) ? $"Consign/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now:dd}/{etc}{index.ToString().PadLeft(3, '0')}{Path.GetExtension(fileName)}"
                    : $"Consign/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now:dd}/{etc}{index.ToString().PadLeft(3, '0')}_{etc2.Split('|')[index - 1]}_{Path.GetExtension(fileName)}";
            } 
            else if (type.Equals("Join"))
            {
                if (fileName.Contains("\\identification\\"))
                {
                    fileName = $"memIdentification/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
                else if (fileName.Contains("\\memBusinessLicenseDoc\\"))
                {
                    fileName = $"memBusinessLicenseDoc/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
                else if (fileName.Contains("\\memBusinessCard\\"))
                {
                    fileName = $"memBusinessCard/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
            }
            else if (type.Equals("Notice") || type.Equals("Main"))
            {
                fileName = $"{type}/{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
            }
            else if (type.Equals("MainMobile") || type.Equals("MainEn") || type.Equals("MainMobileEn"))
            {
                fileName = $"Main/{DateTime.Now:yyyyMMddHHmmss}_{type.Replace("Mobile", "m").Replace("Main", "").Replace("En", "_en")}_{Path.GetExtension(fileName)}";
            }
            else if (type.ToLower().Equals("imgfileupload"))
            {
                fileName = $"{type}/{DateTime.Now:yyyy_MM}/{fileName}";
            }
            else if (type.ToLower().Equals("content"))
            {
                fileName = $"{type}/{DateTime.Now:yyyyMMdd}/{fileName}";
            }
            else
            {
                fileName = $"{type}/{fileName}";
            }
            return fileName;
        }

        public static string GetFileNameKoffice(string type, string fileName, string etc)
        {
            if (type.Equals("Join"))
            {
                if (fileName.Contains("\\identification\\"))
                {
                    fileName = $"memIdentification/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
                else if (fileName.Contains("\\memBusinessLicenseDoc\\"))
                {
                    fileName = $"memBusinessLicenseDoc/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
                else if (fileName.Contains("\\memBusinessCard\\"))
                {
                    fileName = $"memBusinessCard/{etc}/{fileName[(fileName.LastIndexOf('\\') + 1)..]}";
                }
            }
            return fileName;
        }
    }
}
