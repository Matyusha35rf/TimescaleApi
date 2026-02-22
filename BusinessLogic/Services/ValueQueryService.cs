using BusinessLogic.Services.Interfaces;
using BusinessLogic.Models.DTOs;
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
        /// Сортировка: от новых к старым по дате операции
        /// </summary>
        /// <param name="fileName">Имя файла (например, "data.csv")</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список из 10 последних записей или пустой список, если файл не указан</returns>
        public async Task<List<ValueRecordDto>> GetLastTenValuesAsync(string fileName, CancellationToken cancellationToken = default)
        {
            // Защита от пустого имени файла
            if (string.IsNullOrWhiteSpace(fileName))
                return new List<ValueRecordDto>();

            // Получаем данные из репозитория (сущности DataAccess)
            var values = await valuesRepository.GetLastTenByFileNameAsync(fileName, cancellationToken);

            // Преобразуем сущности в DTO для отправки клиенту
            return values.Select(r => new ValueRecordDto
            {
                Id = r.Id,                    // Идентификатор записи (для возможных ссылок)
                FileName = r.FileName,        // Имя файла-источника
                Date = r.Date,                 // Время начала операции (из CSV)
                ExecutionTime = r.ExecutionTime, // Время выполнения в секундах
                Value = r.Value                 // Показатель
            }).ToList();
        }
    }
}