using DataAccess.Models;
using BusinessLogic.Models.DTOs;

namespace BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для запросов к таблице Results
    /// Отвечает за получение интегральных результатов с фильтрацией (МЕТОД 2 ИЗ ТЗ)
    /// </summary>
    public interface IResultQueryService
    {
        /// <summary>
        /// Получение списка результатов с применением фильтров
        /// </summary>
        /// <param name="filter">Параметры фильтрации:
        /// - FileName: имя файла
        /// - FirstOperationDateFrom/To: диапазон дат первой операции
        /// - AverageValueFrom/To: диапазон среднего значения
        /// - AverageExecutionTimeFrom/To: диапазон среднего времени выполнения
        /// </param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Отфильтрованный список результатов</returns>
        Task<List<ResultDto>> GetFilteredResultsAsync(ResultFilter filter, CancellationToken cancellationToken = default);
    }
}