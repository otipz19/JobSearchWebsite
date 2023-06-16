using Microsoft.AspNetCore.Http;

namespace Utility.Interfaces.FileUpload.Document
{
    public interface IDocumentService
    {
        public Task<string> UploadDoc(IFormFile formFile);

        public void DeleteDoc(string webDocPath);
    }
}
