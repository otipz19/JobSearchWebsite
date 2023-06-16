using Microsoft.AspNetCore.Http;

namespace Utility.Interfaces.FileUpload.Image
{
    public interface IImageService
    {
        public Task<string> UploadImage(IFormFile formFile);

        public void DeleteImage(string webImagePath);
    }
}
