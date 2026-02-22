using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Models.DTOs;

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными из CSV файлов (таблица Values)
    /// </summary>
    [ApiController]
    [Route("api/values")]
    public class ValuesController(IValueQueryService valueQueryService) : ControllerBase
    {
        /// <summary>
        /// Получение последних 10 записей для указанного файла (МЕТОД 3 ИЗ ТЗ)
        /// </summary>
        /// <param name="fileName">Имя файла (например, "data.csv")</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список из 10 последних записей файла</returns>
        /// <response code="200">Успешно возвращен список записей</response>
        /// <response code="400">Имя файла не указано</response>
        [HttpGet("{fileName}/last-ten")]
        public async Task<ActionResult<List<ValueRecordDto>>> GetLastTenValues(
            string fileName,
            CancellationToken cancellationToken)
        {
            // Валидация входных параметров
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("Имя файла не может быть пустым");

            // Получение данных через сервис бизнес-логики
            var values = await valueQueryService.GetLastTenValuesAsync(fileName, cancellationToken);

            // Возврат результата в формате DTO (без технических полей)
            return Ok(values);
        }
    }
}