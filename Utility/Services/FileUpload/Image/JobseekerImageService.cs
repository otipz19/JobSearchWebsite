using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Utility.Interfaces.FileUpload.Image;

namespace Utility.Services.FileUpload.Image
{
    public class JobseekerImageService : ImageService, IJobseekerImageService
    {
        private const string WebFolderPath = "\\images\\jobseekers";

        public JobseekerImageService(IWebHostEnvironment environment) : base(environment)
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