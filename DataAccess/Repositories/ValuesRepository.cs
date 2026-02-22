using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Репозиторий для работы с таблицей ValueRecords
    /// </summary>
    internal class ValuesRepository(AppDbContext context) : IValuesRepository
    {
        /// <summary>
        /// Массовое добавление записей в БД
        /// </summary>
        /// <param name="records">Коллекция записей для добавления</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task AddRangeAsync(IEnumerable<ValueRecord> records, CancellationToken cancellationToken = default)
        {
            // Устанавливаем техническое поле CreatedAt для всех записей
            var now = DateTime.UtcNow;
            foreach (var record in records)
            {
                record.CreatedAt = now;
            }

            await context.Values.AddRangeAsync(records, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получение последних 10 записей для указанного файла (МЕТОД 3 ИЗ ТЗ)
        /// Сортировка от новых к старым по дате операции
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список из 10 последних записей</returns>
        public async Task<List<ValueRecord>> GetLastTenByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await context.Values
                .Where(x => x.FileName == fileName)
                .OrderByDescending(x => x.Date)  // новые сверху
                .Take(10)                         // только 10
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Удаление всех записей для указанного файла
        /// Используется при перезаписи файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var recordsToDelete = await context.Values
                .Where(x => x.FileName == fileName)
                .ToListAsync(cancellationToken);

            if (recordsToDelete.Any())
            {
                context.Values.RemoveRange(recordsToDelete);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Сохраняет записи для файла.
        /// Если записи уже существуют - перезаписывает.
        /// </summary>
        /// <param name="records">Коллекция записей для сохранения</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task ReplaceByFileNameAsync(IEnumerable<ValueRecord> records, string fileName, CancellationToken cancellationToken = default)
        {
            // Удаляем старые записи для этого файла (если есть)
            await DeleteByFileNameAsync(fileName, cancellationToken);

            // Добавляем новые записи
            await AddRangeAsync(records, cancellationToken);
        }
    }
}