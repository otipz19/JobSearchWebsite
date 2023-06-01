using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Utility.Interfaces.Image;

namespace Utility.Services.Image
{
    public class CompanyImageService : ImageService, ICompanyImageService
    {
        private const string WebFolderPath = "\\images\\companies";

        public CompanyImageService(IWebHostEnvironment environment) : base(environment)
        {
        }

        public Task<string> UploadImage(IFormFile formFile)
        {
            return base.UploadImage(formFile, WebFolderPath);
        }

        public void DeleteImage(string webImagePath)
        {
            base.DeleteImage(webImagePath, WebFolderPath);
        }
    }
}
