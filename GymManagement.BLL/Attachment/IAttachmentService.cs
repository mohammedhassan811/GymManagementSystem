using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;

namespace GymManagement.BLL.Attachment
{
    public interface IAttachmentService
    {
        Task<Result<string>?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        Result Delete(string fileName, string folderName);
        (Stream stream, string contentType)? GetFile(string fileName, string folderName);

    }
}
