using BusinessLogic.Models;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class FileProcessingService(
        IValueRecordRepository valueRecordRepository,
        IResultRepository resultRepository) : IFileProcessingService  // реализуем интерфейс
    {
        public async Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file)  // точно как в интерфейсе
        {
            // TODO: добавить логику
            
            return new UploadCsvResponse
            {
                Success = true,
                FileName = file.FileName,
                Message = "Файл успешно обработан"
            };
        }
    }
}