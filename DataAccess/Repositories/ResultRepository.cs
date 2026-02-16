using DataAccess.Interfaces;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    internal class ResultRepository(AppDbContext context) : IResultRepository
    {
        public async Task CreateAsync(Result result, CancellationToken cancellationToken = default)
        {
            await context.Results.AddAsync(result, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}