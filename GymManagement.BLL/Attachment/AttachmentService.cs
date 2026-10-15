using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagement.BLL.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; //5 MB
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".png", ".jpeg", ".jpg" };

        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public Result Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            try
            {
                if (!File.Exists(fullPath)) return Result.NotFound("Not found");
                File.Delete(fullPath);
                return Result.Ok();
            }
            catch
            {

                return Result.Fail($"Failed To Deleted Attachment : {fileName}");
            }
            
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                return null;
            var fullPath =Path.Combine(_env.ContentRootPath,folderName,fileName);
            if (!File.Exists(fullPath)) return null;
            var stream = new FileStream(fullPath,FileMode.Open,FileAccess.Read);
            var extension = Path.GetExtension(fullPath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
            return (stream,contentType);
        }

        public async Task<Result<string>?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || !fileStream.CanRead) return Result<string>.Fail("File Stream Is null or Cannot Read");
            if (fileStream.Length == 0) return Result<string>.Fail("Length Cannot be 0 ");
            if (fileStream.Length > _maxFileSize)
            {
                _logger.LogError($"File Rejected : File Too Long {fileStream.Length} bytes");
                return Result<string>.Fail($"File Rejected : File Too Long {fileStream.Length} bytes");
            }

            var extension = Path.GetExtension(fileName);
            if(string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"File Rejected : Extension {extension} is Not Allowed");
                return Result<string>.Fail($"File Rejected : Extension {extension} is Not Allowed\"");
            }
            var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);
            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fs, ct);
                return Result<string>.Ok(storedFileName);
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed To Upload File {fileName}");
                return null;
            }
        }
    }
}
