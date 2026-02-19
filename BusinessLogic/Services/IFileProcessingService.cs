using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public interface IFileProcessingService
    {
        Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
