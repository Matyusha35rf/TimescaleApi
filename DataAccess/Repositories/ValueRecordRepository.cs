using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    internal class ValueRecordRepository(AppDbContext context) : IValueRecordRepository
    {
        public async Task CreateAsync(ValueRecord valueRecord, CancellationToken cancellationToken = default)
        {

            await context.ValueRecords.AddAsync(valueRecord, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
