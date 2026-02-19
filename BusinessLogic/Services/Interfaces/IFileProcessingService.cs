using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileProcessingService
    {
        Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
