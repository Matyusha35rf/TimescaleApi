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
        /// <param name="filter">Параметры фильтрации (имя файла, диапазон дат, значения)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Отфильтрованный список результатов</returns>
        public async Task<List<Result>> GetFilteredResultsAsync(ResultFilter filter, CancellationToken cancellationToken = default)
        {
            // Прокси-метод: просто передаем запрос в репозиторий
            // Вся логика фильтрации уже реализована на уровне доступа к данным
            return await resultsRepository.GetFilteredAsync(filter, cancellationToken);
        }
    }
}