using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Utility.Services.FileUpload.Document
{
    public abstract class DocumentService : BaseFileUploadService
    {
        private const int MaxAllowedDocSizeInBytes = 1024 * 1024 * 50;

        private static readonly IEnumerable<string> AllowedExtensions = new[] { ".doc", ".docx", ".pdf" };

        protected DocumentService(IWebHostEnvironment environment) : base(environment)
        {
        }

        protected async Task<string> UploadDoc(IFormFile formFile, string webFolderPath)
        {
            return await UploadFile(formFile, webFolderPath, AllowedExtensions, MaxAllowedDocSizeInBytes);
        }
    }
}
