using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Сервис для запросов к таблице Values
    /// Отвечает за получение данных из CSV файлов (МЕТОД 3 ИЗ ТЗ)
    /// </summary>
    public class ValueQueryService(IValuesRepository valuesRepository) : IValueQueryService
    {
        /// <summary>
        /// Получение последних 10 записей для указанного файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список из 10 последних записей или пустой список, если файл не указан</returns>
        public async Task<List<ValueRecord>> GetLastTenValuesAsync(string fileName, CancellationToken cancellationToken = default)
        {
            // Защита от пустого имени файла
            if (string.IsNullOrWhiteSpace(fileName))
                return new List<ValueRecord>();

            // Делегируем запрос репозиторию
            return await valuesRepository.GetLastTenByFileNameAsync(fileName, cancellationToken);
        }
    }
}