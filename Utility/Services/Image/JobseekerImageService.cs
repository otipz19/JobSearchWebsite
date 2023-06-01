using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Utility.Interfaces.Image;

namespace Utility.Services.Image
{
    public class JobseekerImageService : ImageService, IJobseekerImageService
    {
        private const string WebFolderPath = "\\images\\jobseekers";

        public JobseekerImageService(IWebHostEnvironment environment) : base(environment)
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