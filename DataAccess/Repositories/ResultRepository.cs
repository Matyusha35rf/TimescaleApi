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

        // Замена (перезапись)
        public async Task ReplaceByFileNameAsync(Result result, string fileName, CancellationToken cancellationToken = default)
        {
            await DeleteByFileNameAsync(fileName, cancellationToken);
            await AddAsync(result, cancellationToken);
        }
    }
}