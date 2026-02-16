using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic
{
    public interface IFileProcessingService
    {
        Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file);
    }
}
