using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace BusinessLogic.Services
{
    public class ValueQueryService(IValuesRepository valuesRepository) : IValueQueryService
    {
        public async Task<List<ValueRecord>> GetLastTenValuesAsync(string fileName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new List<ValueRecord>();

            return await valuesRepository.GetLastTenByFileNameAsync(fileName, cancellationToken);
        }
    }
}