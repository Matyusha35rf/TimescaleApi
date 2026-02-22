using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Models.DTOs; // для UploadCsvResponseDto

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер для загрузки и обработки CSV файлов (МЕТОД 1 ИЗ ТЗ)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CsvFileUploadController(IFileProcessingService fileProcessingService) : ControllerBase
    {
        /// <summary>
        /// Загрузка и обработка CSV файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Результат обработки: успех/ошибка с деталями</returns>
        /// <response code="200">Файл успешно обработан</response>
        /// <response code="400">Ошибка валидации или пустой файл</response>
        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            // Проверка наличия файла
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пуст");

            // Обработка файла через сервис бизнес-логики
            var result = await fileProcessingService.ProcessCsvFileAsync(file);

            // Возврат результата в зависимости от успеха операции
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}