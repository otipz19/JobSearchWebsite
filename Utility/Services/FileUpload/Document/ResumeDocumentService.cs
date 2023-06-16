using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Utility.Interfaces.FileUpload.Document;

namespace Utility.Services.FileUpload.Document
{
    public class ResumeDocumentService : DocumentService, IResumeDocumentService
    {
        private const string WebFolderPath = "\\documents\\resumes";

        public ResumeDocumentService(IWebHostEnvironment environment) : base(environment)
        {
        }

        public async Task<string> UploadDoc(IFormFile formFile)
        {
            return await base.UploadDoc(formFile, WebFolderPath);
        }

        public void DeleteDoc(string webDocPath)
        {
            base.DeleteFile(webDocPath, WebFolderPath);
        }
    }
}
