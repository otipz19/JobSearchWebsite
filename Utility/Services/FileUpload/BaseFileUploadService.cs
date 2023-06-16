using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Utility.Services.FileUpload
{
    public abstract class BaseFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        protected BaseFileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <returns>Path to file that is accessible in browser</returns>
        protected async Task<string> UploadFile(IFormFile formFile, string webFolderPath,
            IEnumerable<string> allowedExtensions, int maxAllowedFileSizeInBytes)
        {
            if (formFile == null || webFolderPath.IsNullOrEmpty())
                throw new ArgumentException("No file to upload");

            string extension = Path.GetExtension(formFile.FileName);
            if (extension == null || !allowedExtensions.Contains(extension))
                throw new ArgumentException("Not allowed format");

            if (formFile.Length <= 0 || formFile.Length > maxAllowedFileSizeInBytes)
                throw new ArgumentException("File size is too big");

            string webFilePath = Path.Join(webFolderPath, Guid.NewGuid().ToString() + extension);
            string fullPath = Path.Join(_environment.WebRootPath, webFilePath);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return webFilePath;
        }

        protected void DeleteFile(string webFilePath, string webFolderPath)
        {
            if (webFilePath.IsNullOrEmpty() || webFolderPath.IsNullOrEmpty())
                throw new ArgumentException();
            string fullPath = Path.Join(_environment.WebRootPath, webFilePath);
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