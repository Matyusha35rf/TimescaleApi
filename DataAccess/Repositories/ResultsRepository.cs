using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Репозиторий для работы с таблицей Results
    /// </summary>
    internal class ResultsRepository(AppDbContext context) : IResultsRepository
    {
        /// <summary>
        /// Добавление нового результата в БД
        /// </summary>
        /// <param name="result">Объект Result для сохранения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task AddAsync(Result result, CancellationToken cancellationToken = default)
        {
            // Устанавливаем техническое поле - время создания записи
            result.CreatedAt = DateTime.UtcNow;

            await context.Results.AddAsync(result, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Получение результата по имени файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Result или null если файл не найден</returns>
        public async Task<Result?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await context.Results
                .FirstOrDefaultAsync(x => x.FileName == fileName, cancellationToken);
        }

        /// <summary>
        /// Получение списка результатов с фильтрацией
        /// Метод 2 из ТЗ - фильтрация по различным параметрам
        /// </summary>
        /// <param name="filter">Объект с параметрами фильтрации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Отфильтрованный список результатов</returns>
        public async Task<List<Result>> GetFilteredAsync(ResultFilter filter, CancellationToken cancellationToken = default)
        {
            var query = context.Results.AsQueryable();

            // Фильтр по имени файла (точное совпадение)
            if (!string.IsNullOrWhiteSpace(filter.FileName))
            {
                query = query.Where(x => x.FileName == filter.FileName);
            }

            // Фильтр по диапазону даты первой операции (нижняя граница)
            if (filter.FirstOperationDateFrom.HasValue)
            {
                query = query.Where(x => x.FirstOperationDate >= filter.FirstOperationDateFrom.Value);
            }

            // Фильтр по диапазону даты первой операции (верхняя граница)
            // Используем < AddDays(1) чтобы включить все записи за указанный день
            if (filter.FirstOperationDateTo.HasValue)
            {
                var toDate = filter.FirstOperationDateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.FirstOperationDate < toDate);
            }

            // Фильтр по среднему значению (нижняя граница)
            if (filter.AverageValueFrom.HasValue)
            {
                query = query.Where(x => x.AverageValue >= filter.AverageValueFrom.Value);
            }

            // Фильтр по среднему значению (верхняя граница)
            if (filter.AverageValueTo.HasValue)
            {
                query = query.Where(x => x.AverageValue <= filter.AverageValueTo.Value);
            }

            // Фильтр по среднему времени выполнения (нижняя граница)
            if (filter.AverageExecutionTimeFrom.HasValue)
            {
                query = query.Where(x => x.AverageExecutionTime >= filter.AverageExecutionTimeFrom.Value);
            }

            // Фильтр по среднему времени выполнения (верхняя граница)
            if (filter.AverageExecutionTimeTo.HasValue)
            {
                query = query.Where(x => x.AverageExecutionTime <= filter.AverageExecutionTimeTo.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Удаление результата по имени файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var result = await context.Results
                .FirstOrDefaultAsync(x => x.FileName == fileName, cancellationToken);

            if (result != null)
            {
                context.Results.Remove(result);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Сохраняет результат для файла.
        /// Если запись уже существует - перезаписывает.
        /// </summary>
        /// <param name="result">Новый результат</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task ReplaceByFileNameAsync(Result result, string fileName, CancellationToken cancellationToken = default)
        {
            // Удаляем старый результат для этого файла
            await DeleteByFileNameAsync(fileName, cancellationToken);
            
            // Добавляем новый результат
            await AddAsync(result, cancellationToken);
        }
    }
}