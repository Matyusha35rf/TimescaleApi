using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IResultsRepository
    {
        // Добавление одного результата
        Task AddAsync(Result result, CancellationToken cancellationToken = default);
        
        // Удаление по имени файла
        Task DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
        
        // Получение по имени файла
        Task<Result?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
        
        // Замена (перезапись)
        Task ReplaceByFileNameAsync(Result result, string fileName, CancellationToken cancellationToken = default);
    }
}