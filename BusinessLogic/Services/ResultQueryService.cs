using BusinessLogic.Models.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Сервис для запросов к таблице Results
    /// Отвечает за получение интегральных результатов с фильтрацией (МЕТОД 2 ИЗ ТЗ)
    /// </summary>
    public class ResultQueryService(IResultsRepository resultsRepository) : IResultQueryService
    {
        /// <summary>
        /// Получение списка результатов с применением фильтров
        /// </summary>
        /// <param name="filter">Параметры фильтрации:
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Отфильтрованный список результатов в формате DTO (без технических полей)</returns>
        public async Task<List<ResultDto>> GetFilteredResultsAsync(ResultFilter filter, CancellationToken cancellationToken = default)
        {
            // Получаем данные из репозитория (сущности DataAccess)
            var results = await resultsRepository.GetFilteredAsync(filter, cancellationToken);

            // Преобразуем сущности в DTO для отправки клиенту
            return results.Select(r => new ResultDto
            {
                Id = r.Id,                                   // Оставляем для возможных ссылок
                FileName = r.FileName,                       // Бизнес-поле
                TimeDeltaSeconds = r.TimeDeltaSeconds,       // Дельта времени в секундах
                FirstOperationDate = r.FirstOperationDate,    // Дата первой операции
                AverageExecutionTime = r.AverageExecutionTime, // Среднее время выполнения
                AverageValue = r.AverageValue,                // Среднее значение
                MedianValue = r.MedianValue,                  // Медиана
                MaxValue = r.MaxValue,                        // Максимальное значение
                MinValue = r.MinValue                         // Минимальное значение
            }).ToList();
        }
    }
}