using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Models.DTOs;
using DataAccess.Models; // для ResultFilter

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с интегральными результатами (таблица Results)
    /// </summary>
    [ApiController]
    [Route("api/results")]
    public class ResultsController(IResultQueryService resultQueryService) : ControllerBase
    {
        /// <summary>
        /// Получение списка результатов с фильтрацией (МЕТОД 2 ИЗ ТЗ)
        /// </summary>
        /// <param name="filter">Параметры фильтрации:
        /// </param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Отфильтрованный список результатов в формате DTO (без технических полей)</returns>
        /// <response code="200">Успешно возвращен список результатов</response>
        [HttpGet]
        public async Task<ActionResult<List<ResultDto>>> GetResults(
            [FromQuery] ResultFilter filter,
            CancellationToken cancellationToken)
        {
            // Получение данных через сервис бизнес-логики
            var results = await resultQueryService.GetFilteredResultsAsync(filter, cancellationToken);

            // Возврат результата в формате DTO (без технических полей)
            return Ok(results);
        }
    }
}