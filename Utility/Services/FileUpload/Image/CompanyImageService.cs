using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Utility.Interfaces.FileUpload.Image;

namespace Utility.Services.FileUpload.Image
{
    public class CompanyImageService : ImageService, ICompanyImageService
    {
        private const string WebFolderPath = "\\images\\companies";

        public CompanyImageService(IWebHostEnvironment environment) : base(environment)
        {
        }

        public async Task<string> UploadImage(IFormFile formFile)
        {
            return await base.UploadImage(formFile, WebFolderPath);
        }

        public void DeleteImage(string webImagePath)
        {
            DeleteFile(webImagePath, WebFolderPath);
        }
    }
}
