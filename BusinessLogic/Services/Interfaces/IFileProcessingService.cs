using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для обработки CSV файлов
    /// Отвечает за загрузку, валидацию и сохранение данных из файлов (МЕТОД 1 ИЗ ТЗ)
    /// </summary>
    public interface IFileProcessingService
    {
        /// <summary>
        /// Обработка CSV файла с данными
        /// Выполняет:
        /// - Чтение и парсинг файла
        /// - Валидацию всех полей согласно ТЗ
        /// - Сохранение записей в таблицу Values
        /// - Расчет и сохранение метрик в таблицу Results
        /// </summary>
        /// <param name="file">Загруженный CSV файл</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат обработки с деталями (успех/ошибка, сообщение)</returns>
        Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}