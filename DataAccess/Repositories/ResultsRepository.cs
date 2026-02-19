using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    internal class ResultsRepository(AppDbContext context) : IResultsRepository
    {
        // Добавление
        public async Task AddAsync(Result result, CancellationToken cancellationToken = default)
        {
            result.CreatedAt = DateTime.UtcNow;

            await context.Results.AddAsync(result, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Удаление по имени файла
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

        // Получение по имени файла
        public async Task<Result?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await context.Results
                .FirstOrDefaultAsync(x => x.FileName == fileName, cancellationToken);
        }

        public async Task<List<Result>> GetFilteredAsync(ResultFilter filter, CancellationToken cancellationToken = default)
        {
            var query = context.Results.AsQueryable();

            // Фильтр по имени файла (частичное совпадение)
            if (!string.IsNullOrWhiteSpace(filter.FileName))
            {
                query = query.Where(x => x.FileName == filter.FileName);
            }

            // Фильтр по диапазону даты первой операции
            if (filter.FirstOperationDateFrom.HasValue)
            {
                query = query.Where(x => x.FirstOperationDate >= filter.FirstOperationDateFrom.Value);
            }

            if (filter.FirstOperationDateTo.HasValue)
            {
                var toDate = filter.FirstOperationDateTo.Value.Date.AddDays(1); // включая конец дня
                query = query.Where(x => x.FirstOperationDate < toDate);
            }

            // Фильтр по среднему значению
            if (filter.AverageValueFrom.HasValue)
            {
                query = query.Where(x => x.AverageValue >= filter.AverageValueFrom.Value);
            }

            if (filter.AverageValueTo.HasValue)
            {
                query = query.Where(x => x.AverageValue <= filter.AverageValueTo.Value);
            }

            // Фильтр по среднему времени выполнения
            if (filter.AverageExecutionTimeFrom.HasValue)
            {
                query = query.Where(x => x.AverageExecutionTime >= filter.AverageExecutionTimeFrom.Value);
            }

            if (filter.AverageExecutionTimeTo.HasValue)
            {
                query = query.Where(x => x.AverageExecutionTime <= filter.AverageExecutionTimeTo.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        // Замена (перезапись)
        public async Task ReplaceByFileNameAsync(Result result, string fileName, CancellationToken cancellationToken = default)
        {
            await DeleteByFileNameAsync(fileName, cancellationToken);
            await AddAsync(result, cancellationToken);
        }
    }
}