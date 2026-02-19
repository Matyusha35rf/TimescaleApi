using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess.Repositories
{
    internal class ValuesRepository(AppDbContext context) : IValuesRepository
    {
        // Массовое добавление
        public async Task AddRangeAsync(IEnumerable<ValueRecord> records, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var record in records)
            {
                record.CreatedAt = now;
            }

            await context.Values.AddRangeAsync(records, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Массовое удаление по имени файла
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

        // Замена (перезапись)
        public async Task ReplaceByFileNameAsync(IEnumerable<ValueRecord> records, string fileName, CancellationToken cancellationToken = default)
        {
            await DeleteByFileNameAsync(fileName, cancellationToken);
            await AddRangeAsync(records, cancellationToken);
        }
    }
}
