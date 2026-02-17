using BusinessLogic.Models;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class FileProcessingService(
        IValueRecordRepository valueRecordRepository,
        IResultRepository resultRepository) : IFileProcessingService  // реализуем интерфейс
    {
        public async Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file)
        {
            var response = new UploadCsvResponse
            {
                FileName = file.FileName,
                Success = true,
                Message = "Файл успешно обработан",
                RowsSaved = 0 // пока 0, позже заполнится
            };

            try
            {
                // логика обработки
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ошибка обработки";
                response.Errors.Add(ex.Message);
                return response;
            }
        }
    }
}