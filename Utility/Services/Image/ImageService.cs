using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Utility.Interfaces.Image;

namespace Utility.Services.Image
{
    public abstract class ImageService
    {
        private const int MaxAllowedImageSizeInBytes = 1024 * 1024 * 20;

        private static readonly IEnumerable<string> AllowedExtensions = new[] { ".jpg", ".png", ".wbep", ".gif" };

        private readonly IWebHostEnvironment _environment;

        protected ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <returns>Path to image that is accessible in browser</returns>
        protected async Task<string> UploadImage(IFormFile formFile, string webFolderPath)
        {
            if(formFile == null || webFolderPath.IsNullOrEmpty())
                throw new ArgumentException("No file to upload");

            string extension = Path.GetExtension(formFile.FileName);
            if(extension == null || !AllowedExtensions.Contains(extension))
                throw new ArgumentException("Not allowed format");

            if(formFile.Length <= 0 || formFile.Length > MaxAllowedImageSizeInBytes)
                throw new ArgumentException("Image size is too big");

            string webFilePath = Path.Join(webFolderPath, Guid.NewGuid().ToString() + extension);
            string fullPath = Path.Join(_environment.WebRootPath, webFilePath);
            using(var stream = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return webFilePath;
        }

        protected void DeleteImage(string webImagePath, string webFolderPath)
        {
            if(webImagePath.IsNullOrEmpty() || webFolderPath.IsNullOrEmpty())
                throw new ArgumentException();
            string fullPath = Path.Join(_environment.WebRootPath, webImagePath);
            FileInfo fileInfo = new FileInfo(fullPath);
            string validFolderPath = Path.Join(_environment.WebRootPath, webFolderPath);
            if (fileInfo.DirectoryName.Equals(validFolderPath) &&
                fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }
    }
}
