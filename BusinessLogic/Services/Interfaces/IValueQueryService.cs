using DataAccess.Models;

namespace BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для запросов к таблице Values
    /// Отвечает за получение данных из CSV файлов (МЕТОД 3 ИЗ ТЗ)
    /// </summary>
    public interface IValueQueryService
    {
        /// <summary>
        /// Получение последних 10 записей для указанного файла
        /// Сортировка от новых к старым по дате операции
        /// </summary>
        /// <param name="fileName">Имя файла (например, "data.csv")</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список из 10 последних записей файла</returns>
        Task<List<ValueRecord>> GetLastTenValuesAsync(string fileName, CancellationToken cancellationToken);
    }
}