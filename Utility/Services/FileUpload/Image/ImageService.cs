using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Utility.Services.FileUpload.Image
{
    public abstract class ImageService : BaseFileUploadService
    {
        private const int MaxAllowedImageSizeInBytes = 1024 * 1024 * 20;

        private static readonly IEnumerable<string> AllowedExtensions = new[] { ".jpg", ".png", ".wbep", ".gif" };

        protected ImageService(IWebHostEnvironment environment) : base(environment)
        {
        }

        /// <returns>Path to image that is accessible in browser</returns>
        protected async Task<string> UploadImage(IFormFile formFile, string webFolderPath)
        {
            return await base.UploadFile(formFile, webFolderPath, AllowedExtensions, MaxAllowedImageSizeInBytes);
        }
    }
}
